using System.Collections.Generic;

namespace Livign.CodeToDesign
{
    record SequenceDiagramMethod(string CallingActor, string CalledActor, string Description)
    {
        public List<SequenceDiagramMethod> CallsToOtherTypes { get; private set; } = new List<SequenceDiagramMethod>();
    }
}
