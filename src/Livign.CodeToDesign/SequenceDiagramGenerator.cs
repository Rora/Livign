using Buildalyzer;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livign.CodeToDesign
{
    public class SequenceDiagramGenerator : ISequenceDiagramGenerator
    {
        private readonly SymbolDisplayFormat _symbolFqnFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);


        /// <summary>
        /// Current impl doesn't support:
        ///   methods that have multiple overloads
        /// </summary>
        public async Task<string> GenerateAsync(string pathToSlnFile, string projectName, string classFullyQualifiedName,
            string methodName, IEnumerable<string> externalAssemblyWhitelistedTypes = null)
        {
            var (compilation, projects) = await LoadAndCompileProjectAsync(pathToSlnFile, projectName).ConfigureAwait(false);
            var typeToAnalyzeSymbol = (ITypeSymbol)compilation.GetTypeByMetadataName(classFullyQualifiedName);
            var methodToAnalyzeSymbol = (IMethodSymbol)typeToAnalyzeSymbol.GetMembers(methodName).Single(m => m is IMethodSymbol);

            var ctx = new Models.SequenceDiagramGeneratorContext
            {
                ClassToScanFullyQualifiedName = classFullyQualifiedName,
                MethodToScan = methodName,
                ExternalAssemblyWhitelistedTypes = externalAssemblyWhitelistedTypes ??= Array.Empty<string>(),
                Compilation = compilation,
                Projects = projects
            };

            var sqRootMethod = new SequenceDiagramMethod(CallingActor: string.Empty, methodToAnalyzeSymbol.ContainingType.Name, methodToAnalyzeSymbol.Name);
            var callStack = new List<IMethodSymbol>();
            AnalyzeCallsToOtherTypesForMethod(ctx, sqRootMethod, methodToAnalyzeSymbol, callStack);

            return GenerateSequenceDiagram(sqRootMethod);
        }

        private static async Task<(Compilation, Models.Project[])> LoadAndCompileProjectAsync(string pathToSlnFile, string projectName)
        {
            var _ = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions); // this line forces a reference so MSBuild loads the assembly in question.
            var workspace = MSBuildWorkspace.Create(
                new Dictionary<string, string>() { { "Configuration", "Debug" }, { "Platform", "AnyCPU" } });
            var sln = await workspace.OpenSolutionAsync(pathToSlnFile).ConfigureAwait(false);
            var project = GetProject(pathToSlnFile, projectName, sln);

            var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
            ValidateCompilation(compilation);

            return (compilation, sln.Projects.Select(p => new Models.Project(p)).ToArray());
        }

        private static Project GetProject(string pathToSlnFile, string projectName, Solution sln)
        {
            var project = sln.Projects.Single(p => p.Name == projectName);

            //Use buildalyzer to overwrite the references since that seems to be missing for netcoreapps and net5.0
            var analyzerManager = new AnalyzerManager(pathToSlnFile);
            var projectAnalyzer = analyzerManager.Projects.Values.Single(p => p.ProjectFile.Name == Path.GetFileName(project.FilePath));
            var analyzerResults = projectAnalyzer.Build().First(); //First because it has 1 entry for each target framework

            project = project
                .WithMetadataReferences(analyzerResults.References.Select(refPath => MetadataReference.CreateFromFile(refPath)));
            return project;
        }

        private static void ValidateCompilation(Compilation compilation)
        {
            var diagNosticsErrorLines = compilation.GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.ToString())
                .ToArray();

            if (diagNosticsErrorLines.Any())
            {
                var diagNosticsErrorLinesStr = string.Join(Environment.NewLine, diagNosticsErrorLines);
                throw new ApplicationException($"The following compilation error(s) occurred: {diagNosticsErrorLinesStr}");
            }
        }

        private void AnalyzeCallsToOtherTypesForMethod(Models.SequenceDiagramGeneratorContext ctx, SequenceDiagramMethod methodAnalyzeResult,
            IMethodSymbol methodToAnalyzeSymbol, IList<IMethodSymbol> callStack)
        {
            if (IsDefinedInExternalAssembly(ctx, methodToAnalyzeSymbol))
            {
                //If we can't find any declaring syntax references then we assume this type and method were declared in an external dll
                return;
            }

            if (callStack.Any(methodSymbol => SymbolEqualityComparer.Default.Equals(methodSymbol, methodToAnalyzeSymbol)))
            {
                //This method call was already found in the callstack, don't analyze this method to prevent an infinite recursive loop
                return;
            }

            callStack.Add(methodToAnalyzeSymbol);

            var typeToAnalyzeSymbol = methodToAnalyzeSymbol.ContainingType;
            var methodToAnalyzeSyntaxTreeRoot = methodToAnalyzeSymbol.DeclaringSyntaxReferences.Single()
                            .SyntaxTree
                            .GetRoot()
                            .DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .Single(n => n.Identifier.ToFullString() == methodToAnalyzeSymbol.Name); //Don't support overloads yet
            var methodToAnalyzeSemanticModel = ctx.Compilation.GetSemanticModel(methodToAnalyzeSyntaxTreeRoot.SyntaxTree);

            var invocationMethodSymbols = methodToAnalyzeSyntaxTreeRoot.DescendantNodes(n => true)
                .OfType<InvocationExpressionSyntax>()
                .Select(node => methodToAnalyzeSemanticModel.GetSymbolInfo(node.Expression).Symbol as IMethodSymbol)
                .Where(symb => symb != null)
                .ToArray();

            foreach (var invocationMethodSymbol in invocationMethodSymbols)
            {
                //We only want to create entries for calls to other types, not calls to the same type.
                if (IsMethodOnSameType(typeToAnalyzeSymbol, invocationMethodSymbol))
                {
                    //We don't want to model this call, but we do want to model calls made by this call
                    AnalyzeCallsToOtherTypesForMethod(ctx, methodAnalyzeResult, invocationMethodSymbol, callStack);
                }
                else if (IsDefinedInExternalAssembly(ctx, invocationMethodSymbol))
                {
                    if (IsCallToExternalAssemblyWhitelisted(ctx, invocationMethodSymbol))
                    {
                        //We want to model the call but (not the calls it makes) if it's whitelisted
                        var sqEntry = new SequenceDiagramMethod(
                           CallingActor: typeToAnalyzeSymbol.Name,
                           CalledActor: invocationMethodSymbol.ContainingType.Name,
                           Description: invocationMethodSymbol.Name
                        );
                        methodAnalyzeResult.CallsToOtherTypes.Add(sqEntry);
                    }
                    else
                    {
                        continue;
                    }
                }
                else //Call to other type within the solution
                {
                    var sqEntry = new SequenceDiagramMethod(
                        CallingActor: typeToAnalyzeSymbol.Name,
                        CalledActor: invocationMethodSymbol.ContainingType.Name,
                        Description: invocationMethodSymbol.Name
                    );
                    methodAnalyzeResult.CallsToOtherTypes.Add(sqEntry);
                    AnalyzeCallsToOtherTypesForMethod(ctx, sqEntry, invocationMethodSymbol, callStack);
                }

            }

            callStack.Remove(methodToAnalyzeSymbol);
        }

        private static bool IsDefinedInExternalAssembly(Models.SequenceDiagramGeneratorContext ctx, IMethodSymbol methodToAnalyzeSymbol)
        {
            var assemblyName = methodToAnalyzeSymbol.ContainingAssembly.Identity.Name;
            return !ctx.Projects.Any(p => p.AssemblyName == assemblyName);
        }

        private static bool IsMethodOnSameType(INamedTypeSymbol typeToAnalyzeSymbol, IMethodSymbol invocationMethodSymbol)
        {
            return SymbolEqualityComparer.Default.Equals(invocationMethodSymbol.ContainingType, typeToAnalyzeSymbol);
        }

        private bool IsCallToExternalAssemblyWhitelisted(Models.SequenceDiagramGeneratorContext ctx, IMethodSymbol invocationMethodSymbol)
        {
            return ctx.ExternalAssemblyWhitelistedTypes.Contains(invocationMethodSymbol.ContainingType.ToDisplayString(_symbolFqnFormat));
        }

        private static string GenerateSequenceDiagram(SequenceDiagramMethod sqRootMethod)
        {
            var diagramBody = sqRootMethod.CallsToOtherTypes.SelectMany(GenerateMermaidJSFor);

            var sequenceDiagramBody = String.Join(Environment.NewLine, diagramBody);
            return String.Format(DiagramTemplates.SequenceDiagramTemplate, sqRootMethod.CalledActor, sequenceDiagramBody);
        }

        private static List<string> GenerateMermaidJSFor(SequenceDiagramMethod sqMethod)
        {
            var results = new List<string>
            {
                $"    {sqMethod.CallingActor}->>{sqMethod.CalledActor}: {sqMethod.Description}"
            };

            if (sqMethod.CallsToOtherTypes.Any())
            {
                results.Add($"    activate {sqMethod.CalledActor}");
                results.AddRange(sqMethod.CallsToOtherTypes.SelectMany(GenerateMermaidJSFor));
                results.Add($"    deactivate {sqMethod.CalledActor}");
            }

            return results;
        }

    }
}
