namespace Rhyous.NuGetPackageUpdater
{
    internal interface INuGetReplacer
    {
        void RepaceInFiles(string directory, string package, string version, string assemblyVersion, string csProjTargetFramework, string packageConfigTargetFramework, string publicKeyToken);
    }
}