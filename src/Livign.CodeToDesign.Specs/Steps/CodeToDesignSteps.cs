using FluentAssertions;
using Livign.CodeToDesign.Specs.Steps.CodeToDesign;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Livign.CodeToDesign.Specs.Steps
{
    [Binding]
    public sealed class CodeToDesignSteps
    {
        private readonly ScenarioContext _ctx;
        private readonly ISequenceDiagramGenerator _sequenceDiagramGenerator;

        public CodeToDesignSteps(ScenarioContext scenarioContext)
        {
            _ctx = scenarioContext;
            _sequenceDiagramGenerator = _ctx.GetRequiredService<ISequenceDiagramGenerator>();
        }

        [When(@"I call Livign\.CodeToDesign with the following parameters")]
        public async Task WhenICallLivign_CodeToDesignWithTheFollowingParameters(Table table)
        {
            var paramsDto = table.CreateInstance<CodeToDesignParamsDto>();
            var slnFileLoc = Path.Combine(Environment.CurrentDirectory, "..", paramsDto.SolutionFile);

            var generatedDiagram = await _sequenceDiagramGenerator.GenerateAsync(slnFileLoc, paramsDto.Project, paramsDto.Class, paramsDto.Method, paramsDto.ExternalAssemblyWhitelistedTypes);
            _ctx.Set(generatedDiagram, ContextKeys.LastGeneratedDiagramKey);
        }

    }
}
