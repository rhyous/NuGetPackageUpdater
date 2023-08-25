using Rhyous.NuGetPackageUpdater;
using System.Collections.Generic;

namespace Rhyous.NuGetPackageUpdater
{
    internal class Settings : ISettings
    {
        /// <summary>
        /// Whether to actually make the change, or whether to just lists the files that would have been changed.
        /// </summary>
        public bool DoNothing { get; set; }

        /// <summary>
        /// The path to TF.exe
        /// </summary>
        public string TFPath { get; set; }

        /// <summary>
        /// Whether to checkout from TFS or not.
        /// </summary>
        public bool CheckoutFromTFS { get; set; }

        /// <summary>
        /// Directories to skip.
        /// </summary>
        public List<string> ExcludeDirs { get; set; }
    }
}
