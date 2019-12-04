using Rhyous.SimpleArgs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rhyous.NuGetPackageUpdater
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            new ArgsManager<ArgsHandler>().Start(args);
        }

        internal static void OnArgumentsHandled()
        {
            var directory = Args.Value("Directory");
            var package = Args.Value("Package");
            var version = Args.Value("Version");
            var publicKeyToken = Args.Value("AssemblyPublicKeyToken");
            var assemblyVersion = Args.Value("AssemblyVersion");
            var csProjTargetFramework = Args.Value("CsprojTargetFramework");
            var packagesConfigTargetFramework = Args.Value("PackagesConfigTargetFramework");

            RepaceInFiles(directory, package, version, assemblyVersion, 
                          csProjTargetFramework, packagesConfigTargetFramework);
        }

        internal static void RepaceInFiles(string directory, string package, string version, string assemblyVersion, string csProjTargetFramework, string packageConfigTargetFramework)
        {
            var filePatterns = new List<string> { ".csproj$", "packages.config$", "web.config$", "app.config$" };
            var filesList = FileFinder.Find(directory, string.Join("|", filePatterns));

            var patternDictionary = new Dictionary<string, List<Replacement>>();
            var projPatterns = new List<Replacement> 
            {
                string.IsNullOrWhiteSpace(csProjTargetFramework) 
                    ? CommonReplacements.GetHintPath(package, version)
                    : CommonReplacements.GetHintPathWithTargetFramework(package, version, csProjTargetFramework), 
                CommonReplacements.GetProjectReference(package, version)
            };
            if (!string.IsNullOrWhiteSpace(assemblyVersion))
                projPatterns.Add(CommonReplacements.GetReferenceInclude(package, assemblyVersion));
            patternDictionary.Add(filePatterns[0], projPatterns);

            var packagesConfigPatterns = new List<Replacement> 
            {
                string.IsNullOrWhiteSpace(csProjTargetFramework) 
                    ? CommonReplacements.GetPackagesConfig(package, version) 
                    : CommonReplacements.GetPackagesConfigWithTargetFramework(package, version, packageConfigTargetFramework)
            };
            patternDictionary.Add(filePatterns[1], packagesConfigPatterns);

            if (!string.IsNullOrWhiteSpace(assemblyVersion))
            {
                var webConfigPatterns = new List<Replacement> { CommonReplacements.GetWebConfig(package, assemblyVersion) };
                patternDictionary.Add(filePatterns[2], webConfigPatterns);
                patternDictionary.Add(filePatterns[3], webConfigPatterns);
            }

            foreach (var pattern in filePatterns)
            {
                if (!patternDictionary.TryGetValue(pattern, out List<Replacement> patterns))
                    continue;
                foreach (var file in filesList.Where(f => Regex.IsMatch(f, pattern, RegexOptions.IgnoreCase)))
                {
                    var fileContent = File.ReadAllText(file);
                    if (ReplaceInString(patterns, ref fileContent))
                        File.WriteAllText(file, fileContent);
                }
            }
        }

        internal static bool ReplaceInString(IEnumerable<Replacement> patterns, ref string text)
        {
            bool patternFoundAndTextNotTheSameAfterReplace = false;
            var originalText = text;
            foreach (var pattern in patterns)
            {
                if (!Regex.IsMatch(text, pattern.Pattern, pattern.RegexOptions))
                    continue;
                text = Regex.Replace(text, pattern.Pattern, pattern.ReplacementPattern, pattern.RegexOptions);
                if (text != originalText)
                    patternFoundAndTextNotTheSameAfterReplace = true;
            }
            return patternFoundAndTextNotTheSameAfterReplace;
        }
    }
}
