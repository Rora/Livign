using Livign.CodeToDesign;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livign.Cli.Commands
{
    internal class GenerateSequenceDiagramCommandHandler
    {
        private readonly ISequenceDiagramGenerator _sequenceDiagramGenerator;
        private readonly IConsole _console;

        public GenerateSequenceDiagramCommandHandler(ISequenceDiagramGenerator sequenceDiagramGenerator, IConsole console)
        {
            _sequenceDiagramGenerator = sequenceDiagramGenerator;
            _console = console;
        }

        internal async Task InvokeAsync(string classFqn, string methodName, string slnFile, string projName)
        {
            var sequenceDiagram = await _sequenceDiagramGenerator.GenerateAsync(slnFile, projName, classFqn, methodName);
            _console.Out.Write(sequenceDiagram);
        }
    }
}
