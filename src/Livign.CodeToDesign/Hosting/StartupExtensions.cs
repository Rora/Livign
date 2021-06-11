using Microsoft.Build.Locator;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livign.CodeToDesign.Hosting
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddCodeToDesign(this IServiceCollection services)
        {
            MSBuildLocator.RegisterDefaults();
            services.AddTransient<ISequenceDiagramGenerator, SequenceDiagramGenerator>();
            return services;
        }
    }
}
