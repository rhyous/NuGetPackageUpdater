using Rhyous.NuGetPackageUpdater.Replacers;
using Rhyous.NuGetPackageUpdater.TFS;
using Rhyous.SimpleArgs;
using System.Threading.Tasks;

namespace Rhyous.NuGetPackageUpdater
{
    internal class Program
    {
        static IProgramRunner _ProgramRunner;

        internal static async Task Main(string[] args)
        {
            // Composition Root
            var argsManager = new ArgsManager<ArgsHandler>();
            IArgsReader argsReader = new ArgsReader();
            ISettings settings = new Settings();
            IStringReplacer stringReplacer = new StringReplacer();
            IFileFinder fileFinder = new FileFinder(settings);
            IFileIO fileIO = new FileIO();
            var tfsCheckout = new TFSCheckout(settings);
            INuGetReplacer nuGetReplacer = new NuGetReplacer(stringReplacer, fileFinder, fileIO, settings, tfsCheckout);
            IPackageConfigInserter packageConfigInserter = new PackageConfigInserter(fileIO);
            ICsprojReferenceWithNuGetReplacer csprojReplacer = new CsprojReferenceWithNuGetReplacer(fileFinder, fileIO, packageConfigInserter, settings);
            var runner = new PostArgsProgram(argsReader, csprojReplacer, nuGetReplacer, settings);
            var starter = new Starter(argsManager, runner);
            await starter.StartAsync(args);
        }

        internal static async void OnArgumentsHandledAsync() => await _ProgramRunner.Run();
    }
}