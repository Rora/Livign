using FluentAssertions;
using Livign.CodeToDesign.Specs.ExpectedMermaidJSOutput;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace Livign.CodeToDesign.Specs.Steps
{
    [Binding]
    public sealed class DiagramAssertionSteps
    {

        private readonly ScenarioContext _ctx;

        public DiagramAssertionSteps(ScenarioContext scenarioContext)
        {
            _ctx = scenarioContext;
        }

        [Then(@"the result should be equal to the '(.*)' entry in the resx")]
        public void ThenTheResultShouldBeEqualToTheEntryInTheResx(string diagramResxKey)
        {
            var expectedDiagram = MermaidJSFiles.ResourceManager.GetString(diagramResxKey, CultureInfo.InvariantCulture);
            _ctx.Get<string>(ContextKeys.LastGeneratedDiagramKey).Should().Be(expectedDiagram);
        }

    }
}
