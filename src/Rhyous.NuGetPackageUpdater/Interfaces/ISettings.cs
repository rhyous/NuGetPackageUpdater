using System.Collections.Generic;

namespace Rhyous.NuGetPackageUpdater
{
    internal interface ISettings
    {
        /// <summary>
        /// Whether to actually make the change, or whether to just lists the files that would have been changed.
        /// </summary>
        bool DoNothing { get; set; }

        /// <summary>
        /// The path to TF.exe
        /// </summary>
        string TFPath { get; set; }

        /// <summary>
        /// Whether to checkout from TFS or not.
        /// </summary>
        bool CheckoutFromTFS { get; set; }

        /// <summary>
        /// Directories to skip.
        /// </summary>
        List<string> ExcludeDirs { get; set; }
    }
}