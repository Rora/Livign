﻿using Buildalyzer;
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
    public class SequenceDiagramGenerator
    {
        public static void Initialize()
        {
            //TODO move this to startup code
            MSBuildLocator.RegisterDefaults();
        }

        /// <summary>
        /// Current impl doesn't support:
        ///   methods that have multiple overloads
        /// </summary>
        public async Task<string> GenerateAsync(string pathToSlnFile, string projectName, string classFullyQualifiedName, string methodName)
        {
            var compilation = await LoadAndCompileProjectAsync(pathToSlnFile, projectName).ConfigureAwait(false);

            var typeToAnalyzeSymbol = (ITypeSymbol)compilation.GetTypeByMetadataName(classFullyQualifiedName);
            var methodToAnalyzeSymbol = (IMethodSymbol)typeToAnalyzeSymbol.GetMembers(methodName).Single(m => m is IMethodSymbol);

            var sqRootMethod = new SequenceDiagramMethod(string.Empty, methodToAnalyzeSymbol.ContainingType.Name, methodToAnalyzeSymbol.Name);
            var callStack = new List<IMethodSymbol>();
            AnalyzeCallsToOtherTypesForMethod(sqRootMethod, compilation, methodToAnalyzeSymbol, callStack);

            return GenerateSequenceDiagram(typeToAnalyzeSymbol, sqRootMethod);
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

        private static void AnalyzeCallsToOtherTypesForMethod(SequenceDiagramMethod methodAnalyzeResult,
            Compilation compilation, IMethodSymbol methodToAnalyzeSymbol, IList<IMethodSymbol> callStack)
        {
            if (!methodToAnalyzeSymbol.DeclaringSyntaxReferences.Any())
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
            var methodToAnalyzeSemanticModel = compilation.GetSemanticModel(methodToAnalyzeSyntaxTreeRoot.SyntaxTree);

            var invocationMethodSymbols = methodToAnalyzeSyntaxTreeRoot.DescendantNodes(n => true)
                .OfType<InvocationExpressionSyntax>()
                .Select(node => methodToAnalyzeSemanticModel.GetSymbolInfo(node.Expression).Symbol as IMethodSymbol)
                .Where(symb => symb != null)
                .ToArray();

            foreach (var invocationMethodSymbol in invocationMethodSymbols)
            {
                SequenceDiagramMethod resultForInvocationsOfInvocatingMethod;

                //We only want to create entries for calls to other types, not calls to the same type.
                if (!SymbolEqualityComparer.Default.Equals(invocationMethodSymbol.ContainingType, typeToAnalyzeSymbol))
                {
                    var sqEntry = new SequenceDiagramMethod(
                        CallingActor: typeToAnalyzeSymbol.Name,
                        CalledActor: invocationMethodSymbol.ContainingType.Name,
                        Description: invocationMethodSymbol.Name
                    );
                    methodAnalyzeResult.CallsToOtherTypes.Add(sqEntry);
                    resultForInvocationsOfInvocatingMethod = sqEntry;
                }
                else
                {
                    resultForInvocationsOfInvocatingMethod = methodAnalyzeResult;
                }

                AnalyzeCallsToOtherTypesForMethod(resultForInvocationsOfInvocatingMethod, compilation, invocationMethodSymbol, callStack);
            }

            callStack.Remove(methodToAnalyzeSymbol);
        }

        private static string GenerateSequenceDiagram(ITypeSymbol typeToAnalyzeSymbol,
            SequenceDiagramMethod sqRootMethod)
        {
            var serializedMethods = sqRootMethod.CallsToOtherTypes.SelectMany(m => SerializeWithInnerMethods(m));

            var sequenceDiagramBody = String.Join(Environment.NewLine,
                serializedMethods.Select(sqEntry => $"    {sqEntry.CallingActor}->>{sqEntry.CalledActor}: {sqEntry.Description}"));
            return String.Format(DiagramTemplates.SequenceDiagramTemplate, typeToAnalyzeSymbol.Name, sequenceDiagramBody);
        }
        private static IEnumerable<SequenceDiagramMethod> SerializeWithInnerMethods(SequenceDiagramMethod sequenceDiagramMethod)
        {
            return new SequenceDiagramMethod[] { sequenceDiagramMethod }
                .Concat(sequenceDiagramMethod.CallsToOtherTypes.SelectMany(SerializeWithInnerMethods));
        }

    }
}
