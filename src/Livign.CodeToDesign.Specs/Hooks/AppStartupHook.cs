using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Livign.CodeToDesign.Specs.Hooks
{
    [Binding]
    public class AppStartupHook
    {
        [BeforeTestRun]
        public static void Startup()
        {
            SequenceDiagramGenerator.Initialize();
        }
    }
}
