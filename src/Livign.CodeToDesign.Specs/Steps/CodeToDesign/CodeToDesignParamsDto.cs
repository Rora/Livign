using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livign.CodeToDesign.Specs.Steps.CodeToDesign
{
    public class CodeToDesignParamsDto
    {
        public string SolutionFile { get; set; }
        public string Project { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string ExternalAssemblyWhitelistedTypesStr { get; set; }
        public IEnumerable<string> ExternalAssemblyWhitelistedTypes => string.IsNullOrEmpty(ExternalAssemblyWhitelistedTypesStr) 
            ? null 
            : ExternalAssemblyWhitelistedTypesStr.Split(",").Select(s => s.Trim()).ToArray();
    }
}
