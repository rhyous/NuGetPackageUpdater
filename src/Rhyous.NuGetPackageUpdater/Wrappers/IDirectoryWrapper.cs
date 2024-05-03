namespace Rhyous.NuGetPackageUpdater.Wrappers
{
    /// <summary>
    /// Interface for wrapping <see cref="System.IO.Directory"/>
    /// </summary>
    public interface IDirectoryWrapper
    {
        string[] GetFiles(string path);
        string[] GetDirectories(string path);
    }
}
