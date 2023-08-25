using System.Threading.Tasks;

namespace Rhyous.NuGetPackageUpdater
{
    internal interface ICsprojReferenceWithNuGetReplacer
    {
        Task ReplaceInFilesAsync(string directory, string csprojReferenceToReplace, string replacementSnippetFile, string packagesConfigSnippetFile);
    }
}