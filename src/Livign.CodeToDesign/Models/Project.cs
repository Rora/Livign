namespace Livign.CodeToDesign.Models
{
    public class Project
    {
        public Project(Microsoft.CodeAnalysis.Project p)
        {
            AssemblyName = p.AssemblyName;
        }

        public string AssemblyName { get; init; }
    }
}