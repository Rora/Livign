using Livign.CodeToDesign.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Livign.CodeToDesign.Specs.Hooks
{
    [Binding]
    public class DIHooks
    {
        private static ServiceProvider _serviceProvider;
        private IServiceScope _scenarioDIScope;

        [BeforeTestRun]
        public static void Startup()
        {
            var services = new ServiceCollection();
            services.AddCodeToDesign();
            _serviceProvider = services.BuildServiceProvider();
        }

        [BeforeScenario]
        public void SetDIForScenario(ScenarioContext scenarioContext)
        {
            _scenarioDIScope = _serviceProvider.CreateScope();
            scenarioContext.SetServiceProvider(_scenarioDIScope.ServiceProvider);
        }

        [AfterScenario]
        public void DisposeDIForScenario()
        {
            _scenarioDIScope.Dispose();
        }

        [AfterTestRun]
        public static void DisposeDI()
        {
            _serviceProvider.Dispose();
        }
    }
}
