using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Rhyous.SimpleArgs;

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
            var assemblyVersion = Args.Value("AssemblyVersion");

            RepaceInFiles(directory, package, version, assemblyVersion);
        }

        internal static void RepaceInFiles(string directory, string package, string version, string assemblyVersion)
        {
            var filesList = FileFinder.Find(directory, ".csproj$|packages.config$");
            var projPatterns = new List<Replacement> { CommonReplacements.GetHintPath(package, version) };
            if (!string.IsNullOrWhiteSpace(assemblyVersion))
                projPatterns.Add(CommonReplacements.GetReferenceInclude(package, assemblyVersion));
            var packagesConfigPattern = new List<Replacement> { CommonReplacements.GetPackagesConfig(package, version) };

            foreach (var file in filesList)
            {
                var fileContent = File.ReadAllText(file);
                IEnumerable<Replacement> patterns = file.EndsWith(".csproj", StringComparison.CurrentCultureIgnoreCase) ? projPatterns : packagesConfigPattern;
                if (ReplaceInString(patterns, ref fileContent))
                    File.WriteAllText(file, fileContent);
            }
        }

        internal static bool ReplaceInString(IEnumerable<Replacement> patterns, ref string text)
        {
            bool patternFound = false;
            foreach (var pattern in patterns)
            {
                if (!Regex.IsMatch(text, pattern.Pattern, RegexOptions.Multiline))
                    continue;
                patternFound = true;
                text = Regex.Replace(text, pattern.Pattern, pattern.ReplacementPattern, RegexOptions.Multiline);
            }
            return patternFound;
        }
    }
}
