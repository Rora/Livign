using Livign.Cli.Commands;
using Livign.CodeToDesign.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace Livign.Cli
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = CreateCommandStructure();

            var cmdBuilder = new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .UseHost(hostBuilder =>
                {
                    hostBuilder.ConfigureServices(ConfigureServices);
                    hostBuilder.UseConsoleLifetime();
                })
                .RegisterWithDotnetSuggest()
                .CancelOnProcessTermination()
                .UseHelp();

            return await cmdBuilder
                .Build()
                .InvokeAsync(args);
        }

        private static RootCommand CreateCommandStructure()
        {
            var rootCommand = new RootCommand("Livign: Living design documentation");
            var generateCommand = new Command("generate");
            generateCommand.HasAlias("gen");
            rootCommand.AddCommand(generateCommand);

            var generateSequenceDiagramCommand = CreateGenerateSequenceDiagramCommand();

            generateCommand.AddCommand(generateSequenceDiagramCommand);

            return rootCommand;
        }

        private static Command CreateGenerateSequenceDiagramCommand()
        {
            var generateSequenceDiagramCommand = new Command("sequence-diagram");
            generateSequenceDiagramCommand.HasAlias("sq");
            generateSequenceDiagramCommand.AddArgument(
                new Argument<string>("class") { Description = "Fully qualified classname" });
            generateSequenceDiagramCommand.AddArgument(
                new Argument<string>("method") { Description = "Method to generate sequence diagram for" });
            generateSequenceDiagramCommand.AddOption(
                new Option<string>(new[] { "--solutionFile", "--sln" }, "Path to the solution file to scan") { Required = true });
            generateSequenceDiagramCommand.AddOption(
                new Option<string>(new[] { "--projectName", "--prj" }, "Project to scan") { Required = true });

            generateSequenceDiagramCommand
                .Handler = CommandHandler.Create<string, string, string, string, IHost>((@class, method, solutionFile, projectName, host) =>
                {
                    return host.Services.GetRequiredService<GenerateSequenceDiagramCommandHandler>()
                        .InvokeAsync(@class, method, solutionFile, projectName);
                });

            return generateSequenceDiagramCommand;
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddSingleton<IConsole, SystemConsole>();
            services.AddTransient<GenerateSequenceDiagramCommandHandler, GenerateSequenceDiagramCommandHandler>();
            services.AddCodeToDesign();
        }
    }
}
