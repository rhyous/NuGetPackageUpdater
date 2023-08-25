using Rhyous.StringLibrary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Rhyous.NuGetPackageUpdater.TFS
{
    internal class TFSCheckout : ITFSCheckout
    {
        const int MaxArgumentCharacterLength = 2000;
        private const string BeginningArgs = "vc checkout";
        private readonly ISettings _Settings;
        private string TFPath => _Settings.TFPath.Quote();

        public TFSCheckout(ISettings settings)
        {
            _Settings = settings;
        }

        public void Checkout(IEnumerable<string> filesToCheckout)
        {
            var args = BeginningArgs;
            foreach (var file in filesToCheckout)
            {
                var quotedFile = file.Quote();
                if (TFPath.Length + args.Length + 2 + quotedFile.Length < MaxArgumentCharacterLength)
                {
                   args += " " + quotedFile;
                }
                else
                {
                    RunTFProcess(args);
                    args = $"{BeginningArgs} {quotedFile}"; // Start over
                }
            }
            // Process the final group
            RunTFProcess(args);
        }

        private void RunTFProcess(string args)
        {
            //Checkout file
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo(TFPath.Quote(), args);
            proc.StartInfo.UseShellExecute = false;
            if (proc.Start())
            { }
            else
            { }
            proc.WaitForExit();
        }
    }
}
