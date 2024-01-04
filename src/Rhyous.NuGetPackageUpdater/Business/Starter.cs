using Rhyous.NuGetPackageUpdater;
using Rhyous.SimpleArgs;
using Rhyous.StringLibrary;
using System.Linq;
using System.Threading.Tasks;

namespace Rhyous.NuGetPackageUpdater
{
    internal class Starter
    {
        private readonly IArgsManager _ArgsManager;
        private readonly IProgramRunner _RunProgram;

        public Starter(IArgsManager argsManager,
                       IProgramRunner runProgram)
        {
            _ArgsManager = argsManager;
            _RunProgram = runProgram;
        }

        public async Task StartAsync(string[] args)
        {
            _ArgsManager.Start(args);
            await _RunProgram.Run();
        }
    }

    public interface IProgramRunner
    {
        Task Run();
    }

    internal class PostArgsProgram : IProgramRunner
    {
        private readonly IArgsReader _ArgsReader;
        private readonly ISettings _Settings;
        private readonly ICsprojReferenceWithNuGetReplacer _CsprojReferenceWithNuGetReplacer;
        private readonly INuGetReplacer _NuGetReplacer;

        public PostArgsProgram(IArgsReader argsReader,
                               ICsprojReferenceWithNuGetReplacer csprojReferenceWithNuGetReplacer,
                               INuGetReplacer nuGetReplacer,
                               ISettings settings)
        {
            _ArgsReader = argsReader;
            _CsprojReferenceWithNuGetReplacer = csprojReferenceWithNuGetReplacer;
            _NuGetReplacer = nuGetReplacer;
            _Settings = settings;
        }
        public async Task Run()
        {
            _Settings.DoNothing = _ArgsReader.GetValue("DoNothing").To<bool>();
            _Settings.TFPath = _ArgsReader.GetValue("TFdotExePath").To<string>();
            _Settings.CheckoutFromTFS = _ArgsReader.GetValue("TFSCheckout").To<bool>();
            _Settings.ExcludeDirs = _ArgsReader.GetValue("ExcludeDirectories")?.Split(';', System.StringSplitOptions.RemoveEmptyEntries)?.ToList();
            var directory = _ArgsReader.GetValue("Directory");
            var replaceCsprojReference = _ArgsReader.GetValue("ReplaceCsprojReference").To<bool>();

            if (replaceCsprojReference)
            {
                var csprojReferenceToReplace = _ArgsReader.GetValue("CsprojReferenceToReplace");
                var csprojReferenceReplacementSnippetFile = _ArgsReader.GetValue("CsprojReferenceReplacementSnippetFile");
                var packagesConfigSnippetFile = _ArgsReader.GetValue("PackagesConfigSnippetFile");
                await _CsprojReferenceWithNuGetReplacer.ReplaceInFilesAsync(directory, csprojReferenceToReplace, csprojReferenceReplacementSnippetFile, packagesConfigSnippetFile);
            }
            else
            {
                var package = _ArgsReader.GetValue("Package");
                var version = _ArgsReader.GetValue("Version");
                var publicKeyToken = _ArgsReader.GetValue("AssemblyPublicKeyToken");
                var assemblyVersion = _ArgsReader.GetValue("AssemblyVersion");
                var csProjTargetFramework = _ArgsReader.GetValue("CsprojTargetFramework");
                var packagesConfigTargetFramework = _ArgsReader.GetValue("PackagesConfigTargetFramework");
                _NuGetReplacer.RepaceInFiles(directory, package, version, assemblyVersion, csProjTargetFramework, packagesConfigTargetFramework, publicKeyToken);
            }
        }
    }
}