using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Livign.CodeToDesign.Specs
{
    public static class SpecflowExtensions
    {
        public static void SetServiceProvider(this ScenarioContext scenarioContext, IServiceProvider serviceProvider)
        {
            scenarioContext["ServiceProvider"] = serviceProvider;
        }

        public static IServiceProvider GetServiceProvider(this ScenarioContext scenarioContext)
        {
            return (IServiceProvider) scenarioContext["ServiceProvider"];
        }

        public static T GetRequiredService<T>(this ScenarioContext scenarioContext)
        {
            return scenarioContext.GetServiceProvider().GetRequiredService<T>();
        }
    }
}
