using Rhyous.NuGetPackageUpdater.Models;
using System.Collections.Generic;

namespace Rhyous.NuGetPackageUpdater
{
    internal interface IStringReplacer
    {
        bool ReplaceInString(IEnumerable<Replacement> patterns, ref string text);
    }
}
