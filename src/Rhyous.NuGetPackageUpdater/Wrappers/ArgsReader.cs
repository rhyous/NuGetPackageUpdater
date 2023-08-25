using Rhyous.SimpleArgs;

namespace Rhyous.NuGetPackageUpdater
{
    internal class ArgsReader : IArgsReader
    {
        public string GetValue(string argKey) => Args.Value(argKey);
    }
}