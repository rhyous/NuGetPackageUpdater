using Rhyous.NuGetPackageUpdater.Models;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rhyous.NuGetPackageUpdater.Replacers
{
    public class CommonReplacements
    {
        public const string VersionPattern = "(?:[0-9]+\\.)+[0-9]+"; // ?: to make it a non-capturing group

        public static Replacement GetHintPath(string package, string version)
        {
            return new Replacement
            {
                Pattern = $@"(<HintPath>.*packages\\{package}.){VersionPattern}((?:[^<]*)</HintPath>)",
                ReplacementPattern = $@"${{1}}{version}${{2}}",
                RegexOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase
            };
        }

        public static Replacement GetHintPathWithTargetFramework(string package, string version, string targetFramework)
        {
            return new Replacement
            {
                Pattern = $@"(<HintPath>.*packages\\{package}.){VersionPattern}\\lib\\[^\\]+((?:[^<]*)</HintPath>)",
                ReplacementPattern = $@"${{1}}{version}\lib\{targetFramework}${{2}}",
                RegexOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase
            };
        }

        public static Replacement GetProjectReference(string package, string version)
        {
            return new Replacement
            {
                Pattern = $@"(<PackageReference Include=""{package}"" Version=""){VersionPattern}(""[^>]*>)",
                ReplacementPattern = $@"${{1}}{version}${{2}}",
                RegexOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase
            };
        }

        public static Replacement GetReferenceInclude(string package, string assemblyVersion)
        {
            return new Replacement
            {
                Pattern = $"(<Reference Include=\"{package}, Version=){VersionPattern}(,)",
                ReplacementPattern = $"${{1}}{assemblyVersion}${{2}}",
                RegexOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase
            };
        }
        public static Replacement GetPackagesConfig(string package, string version)
        {
            return new Replacement
            {
                Pattern = $"(<package\\s+id=\"{package}\"\\s+version=\"){VersionPattern}(\"\\s+targetFramework=\"[^\"]+\"\\s*/>)",
                ReplacementPattern = $"${{1}}{version}${{2}}",
                RegexOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase
            };
        }

        public static Replacement GetPackagesConfigWithTargetFramework(string package, string version, string targetFramework)
        {
            return new Replacement
            {
                Pattern = $"(<package\\s+id=\"{package}\"\\s+version=\"){VersionPattern}(\"\\s+targetFramework=\")[^\"]*(\"\\s*/>)",
                ReplacementPattern = $"${{1}}{version}${{2}}{targetFramework}${{3}}",
                RegexOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase
            };
        }

        public static Replacement GetWebConfig(string package, string assemblyVersion)
        {
            return new Replacement
            {
                Pattern = $"(<assemblyIdentity\\s+name=\"Newtonsoft.Json\")"                // Group 1
                        + $"([^>]+)"                                                        // Group 2 any attribute
                        + $"(publicKeyToken=\")"                                            // Group 3
                        + "([^\\\"]+)"                                                      // Group 4 The existing publicKeyToken
                        + $"(\\\"[^>]+>)"                                                   // Group 5 The rest and the next line and whitespace if there is any
                        + $"(\\s*<!--[^>]*>)*"                                              // Group 6 Any number of comments
                        + $"(\\s*)"                                                         // Group 7 The rest and the next line and whitespace if there is any
                        + $"(<bindingRedirect\\s+oldVersion=\\\"0.0.0.0-)"                  // Group 8 everything up to the previous version.
                        + $"(?:[0-9]+\\.)+[0-9]+"                                           // Not a group as we don't want the previous version
                        + $"(\\\"\\s+newVersion=\\\")"                                      // Group 9 everything up to the new version.
                        + $"(?:[0-9]+\\.)+[0-9]+"                                           // Unumbered group - The new version.
                        + $"(\\\"\\s*/>)",                                                  // Group 10 Everything after
                ReplacementPattern = $"${{1}}${{2}}${{3}}${{4}}${{5}}${{6}}${{7}}${{8}}{assemblyVersion}${{9}}{assemblyVersion}${{10}}",
                RegexOptions = RegexOptions.None | RegexOptions.IgnoreCase
            };
        }

        public static Replacement GetWebConfig(string package, string assemblyVersion, string publicKeyToken)
        {
            return new Replacement
            {
                //Pattern = "(<assemblyIdentity\\s+name=\"Newtonsoft.Json\")([^>]+)(publicKeyToken=\")[^\\\"]+\\\"([^>]+>)(\\s*<!--[^>]*>)*(\\s*)(<bindingRedirect\\s+oldVersion=\\\"0.0.0.0-)(?:[0-9]+\\.)+[0-9]+(\\\"\\s+newVersion=\\\")(?:[0-9]+\\.)+[0-9](\\\"\\s*/>)",
                Pattern = $"(<assemblyIdentity\\s+name=\"Newtonsoft.Json\")"                // Group 1
                        + $"([^>]+)"                                                        // Group 2 any attribute
                        + $"(publicKeyToken=\")"                                            // Group 3
                        + $"([^\\\"]+)"                                                      // Group 4 The existing publicKeyToken
                        + $"(\\\"[^>]+>)"                                                   // Group 5 The rest and the next line and whitespace if there is any
                        + $"(\\s*<!--[^>]*>)*"                                              // Group 6 Any number of comments
                        + $"(\\s*)"                                                         // Group 7 The rest and the next line and whitespace if there is any
                        + $"(<bindingRedirect\\s+oldVersion=\\\"0.0.0.0-)"                  // Group 8 everything up to the previous version.
                        + $"(?:[0-9]+\\.)+[0-9]+"                                           // Not a group as we don't want the previous version
                        + $"(\\\"\\s+newVersion=\\\")"                                      // Group 9 everything up to the new version.
                        + $"(?:[0-9]+\\.)+[0-9]+"                                           // Unumbered group - The new version.
                        + $"(\\\"\\s*/>)",                                                  // Group 10 Everything after
                ReplacementPattern = $"${{1}}${{2}}${{3}}{publicKeyToken}${{5}}${{6}}${{7}}${{8}}{assemblyVersion}${{9}}{assemblyVersion}${{10}}",
                RegexOptions = RegexOptions.None | RegexOptions.IgnoreCase
            };
        }
    }
}