using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livign.CodeToDesign.Models
{
    internal class SequenceDiagramGeneratorContext
    {
        public Compilation Compilation { get; set; }
        public Models.Project[] Projects { get; set; }
        public string ClassToScanFullyQualifiedName { get; internal set; }
        public string MethodToScan { get; internal set; }
        public IEnumerable<string> ExternalAssemblyWhitelistedTypes { get; internal set; }
    }
}
