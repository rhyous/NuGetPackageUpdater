namespace Rhyous.NuGetPackageUpdater
{
    internal interface IPackageConfigInserter
    {
        void Insert(string packagesConfigPath, string snippetFile);
    }
}