using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Rhyous.NuGetPackageUpdater.Wrappers
{
    /// <summary>
    /// An implementation of <see cref="IDirectoryWrapper"/>, wrapping the static <see cref="Directory"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DirectoryWrapper : IDirectoryWrapper
    {
        /// <inheritdoc cref="Directory.GetFiles(string)"/>
        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        /// <inheritdoc cref="Directory.GetDirectories(string)"
        public string[] GetDirectories(string path) 
        { 
            return Directory.GetDirectories(path); 
        }
    }
}
