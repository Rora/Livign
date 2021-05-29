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

        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext _ctx;

        public CodeToDesignSteps(ScenarioContext scenarioContext)
        {
            _ctx = scenarioContext;
        }

        [When(@"I call Livign\.CodeToDesign with the following parameters")]
        public async Task WhenICallLivign_CodeToDesignWithTheFollowingParameters(Table table)
        {
            var paramsDto = table.CreateInstance<CodeToDesignParamsDto>();
            var projectLoc = Path.Combine(Environment.CurrentDirectory, "..", paramsDto.Project);

            var sut = new SequenceDiagramGenerator();
            var generatedDiagram = await sut.GenerateAsync(projectLoc, paramsDto.Class, paramsDto.Method);
            _ctx.Set(generatedDiagram, ContextKeys.LastGeneratedDiagramKey);
        }

    }
}
