using System.Threading.Tasks;

namespace Livign.CodeToDesign
{
    public interface ISequenceDiagramGenerator
    {
        Task<string> GenerateAsync(string pathToSlnFile, string projectName, string classFullyQualifiedName, string methodName);
    }
}