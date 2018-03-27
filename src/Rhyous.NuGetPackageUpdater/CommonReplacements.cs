namespace Rhyous.NuGetPackageUpdater
{
    public class CommonReplacements
    {
        public static Replacement GetHintPath(string package, string version)
        {
            return new Replacement
            {
                Pattern = $@"(<HintPath>[.\\]*packages\\{package}.)(?:[0-9]+\.)+[0-9]+((?:[^<]*)</HintPath>)",
                ReplacementPattern = $@"${{1}}{version}${{2}}"
            };
        }

        public static Replacement GetReferenceInclude(string package, string assemblyVersion)
        {
            return new Replacement
            {
                Pattern = $"(<Reference Include=\"{package}, Version=)(?:[0-9]+\\.)+[0-9]+(,)",
                ReplacementPattern = $"${{1}}{assemblyVersion}${{2}}"
            };
        }
        public static Replacement GetPackagesConfig(string package, string version)
        {
            return new Replacement
            {
                Pattern = $"(<package id=\"{package}\" version=\")(?:[0-9]+\\.)+[0-9]+(\" targetFramework=\"net461\" />)",
                ReplacementPattern = $"${{1}}{version}${{2}}"
            };
        }
    }
}
