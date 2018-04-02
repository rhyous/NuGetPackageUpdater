using System.Text.RegularExpressions;

namespace Rhyous.NuGetPackageUpdater
{
    public class CommonReplacements
    {
        public const string VersionPattern = "(?:[0-9]+\\.)+[0-9]+"; // ?: to make it a non-capturing group

        public static Replacement GetHintPath(string package, string version)
        {
            return new Replacement
            {
                Pattern = $@"(<HintPath>[.\\]*packages\\{package}.){VersionPattern}((?:[^<]*)</HintPath>)",
                ReplacementPattern = $@"${{1}}{version}${{2}}",
                RegexOptions = RegexOptions.Multiline
            };
        }

        public static Replacement GetReferenceInclude(string package, string assemblyVersion)
        {
            return new Replacement
            {
                Pattern = $"(<Reference Include=\"{package}, Version=){VersionPattern}(,)",
                ReplacementPattern = $"${{1}}{assemblyVersion}${{2}}",
                RegexOptions = RegexOptions.Multiline
            };
        }
        public static Replacement GetPackagesConfig(string package, string version)
        {
            return new Replacement
            {
                Pattern = $"(<package\\s+id=\"{package}\"\\s+version=\"){VersionPattern}(\"\\s+targetFramework=\"net461\"\\s*/>)",
                ReplacementPattern = $"${{1}}{version}${{2}}",
                RegexOptions = RegexOptions.Multiline
        };
        }

        public static Replacement GetWebConfig(string package, string assemblyVersion)
        {
            return new Replacement
            {
                Pattern = $"(\\s*<dependentAssembly>\\s*<assemblyIdentity\\s+name=\"{package}\"[^>]+>\\s*<bindingRedirect\\s+oldVersion=\"0.0.0.0-){VersionPattern}(\"\\s+newVersion=\"){VersionPattern}(\"\\s*/>\\s*</dependentAssembly>)",
                ReplacementPattern = $"${{1}}{assemblyVersion}${{2}}{assemblyVersion}${{3}}",
                RegexOptions = RegexOptions.None
            };
        }

        public static Replacement GetWebConfig(string package, string version, string publicKeyToken)
        {
            return new Replacement
            {
                Pattern = $"(\\s*<assemblyIdentity\\s+name=\"{package}\"\\s+publicKeyToken=\")[^\"]+(\"\\s+culture=\"neutral\"\\s*/>\\s*<bindingRedirect oldVersion=\"0.0.0.0-){VersionPattern}(\"\\s+newVersion=\"){VersionPattern}(\"\\s*/>)",
                ReplacementPattern = $"${{1}}{publicKeyToken}{{2}}{version}${{3}}{version}${{4}}",
                RegexOptions = RegexOptions.None
            };
        }
    }
}