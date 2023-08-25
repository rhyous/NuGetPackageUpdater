using System.Collections.Generic;

namespace Rhyous.NuGetPackageUpdater
{
    public interface IFileFinder
    {
        IList<string> Find(string directory, string pattern);
    }
}