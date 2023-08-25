using System.Collections.Generic;

namespace Rhyous.NuGetPackageUpdater
{
    internal interface ITFSCheckout
    {
        void Checkout(IEnumerable<string> filesToCheckout);
    }
}