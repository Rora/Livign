using Buildalyzer;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livign.CodeToDesign
{
    public class SequenceDiagramGenerator
    {
        public static void Initialize()
        {
            //TODO move this to startup code
            MSBuildLocator.RegisterDefaults();
        }

        /// <summary>
        /// Current impl doesn't support:
        ///   .net 5 projects - for some reason the msbuild workspace won't load references for .net5
        ///   methods that have multiple overloads
        /// </summary>
        public async Task<string> GenerateAsync(string pathToSlnFile, string projectName, string classFullyQualifiedName, string methodName)
        {
            var compilation = await LoadAndCompileProjectAsync(pathToSlnFile, projectName).ConfigureAwait(false);
            var cnt = compilation.References.Count(); //TODO check why this is empty

            var typeToAnalyzeSymbol = (ITypeSymbol)compilation.GetTypeByMetadataName(classFullyQualifiedName);
            var methodToAnalyzeSymbol = (IMethodSymbol)typeToAnalyzeSymbol.GetMembers(methodName).Single(m => m is IMethodSymbol);

            var sqEntries = CreateSQEntriesForMethod(compilation, methodToAnalyzeSymbol);

            return GenerateSequenceDiagram(typeToAnalyzeSymbol, sqEntries);
        }

        private static async Task<Compilation> LoadAndCompileProjectAsync(string pathToSlnFile, string projectName)
        {
            var _ = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions); // this line forces a reference so MSBuild loads the assembly in question.
            var workspace = MSBuildWorkspace.Create(
                new Dictionary<string, string>() { { "Configuration", "Debug" }, { "Platform", "AnyCPU" } });
            var sln = await workspace.OpenSolutionAsync(pathToSlnFile).ConfigureAwait(false);
            var project = GetProject(pathToSlnFile, projectName, sln);

            var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
            ValidateCompilation(compilation);

            return compilation;
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

        private static List<SequenceDiagramEntry> CreateSQEntriesForMethod(Compilation compilation, IMethodSymbol methodToAnalyzeSymbol)
        {
            if(!methodToAnalyzeSymbol.DeclaringSyntaxReferences.Any())
            {
                //If we can't find any declaring syntax references then we assume this type and method were declared in an external dll
                return new List<SequenceDiagramEntry>();
            }

            var typeToAnalyzeSymbol = methodToAnalyzeSymbol.ContainingType;
            var methodToAnalyzeSyntaxTreeRoot = methodToAnalyzeSymbol.DeclaringSyntaxReferences.Single()
                            .SyntaxTree
                            .GetRoot()
                            .DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .Single(n => n.Identifier.ToFullString() == methodToAnalyzeSymbol.Name); //Don't support overloads yet
            var methodToAnalyzeSemanticModel = compilation.GetSemanticModel(methodToAnalyzeSyntaxTreeRoot.SyntaxTree);

            var invocationMethodSymbols = methodToAnalyzeSyntaxTreeRoot.DescendantNodes(n => true)
                .OfType<InvocationExpressionSyntax>()
                .Select(node => methodToAnalyzeSemanticModel.GetSymbolInfo(node.Expression).Symbol as IMethodSymbol)
                .Where(symb => symb != null)
                .ToArray();

            var sqEntries = new List<SequenceDiagramEntry>();

            foreach (var invocationMethodSymbol in invocationMethodSymbols)
            {
                if (!SymbolEqualityComparer.Default.Equals(invocationMethodSymbol.ContainingType, typeToAnalyzeSymbol))
                {
                    sqEntries.Add(new SequenceDiagramEntry(
                        CallingActor: typeToAnalyzeSymbol.Name,
                        CalledActor: invocationMethodSymbol.ContainingType.Name,
                        Description: invocationMethodSymbol.Name
                    ));
                }

                var sqEntriesForInvokingMethods = CreateSQEntriesForMethod(compilation, invocationMethodSymbol);
                sqEntries.AddRange(sqEntriesForInvokingMethods);
            }

            return sqEntries;
        }

        private static string GenerateSequenceDiagram(ITypeSymbol typeToAnalyzeSymbol, List<SequenceDiagramEntry> sqEntries)
        {
            var sequenceDiagramBody = String.Join(Environment.NewLine,
                sqEntries.Select(sqEntry => $"    {sqEntry.CallingActor}->>{sqEntry.CalledActor}: {sqEntry.Description}"));
            return String.Format(DiagramTemplates.SequenceDiagramTemplate, typeToAnalyzeSymbol.Name, sequenceDiagramBody);
        }
    }
}
