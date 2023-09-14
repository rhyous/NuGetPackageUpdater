using Rhyous.NuGetPackageUpdater;
using Rhyous.NuGetPackageUpdater.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rhyous.NuGetPackageUpdater.Replacers
{
    internal class NuGetReplacer : INuGetReplacer
    {
        private readonly IStringReplacer _stringReplacer;
        private readonly IFileFinder _FileFinder;
        private readonly IFileIO _FileIO;
        private readonly ISettings _Settings;
        private readonly ITFSCheckout _TFSCheckout;

        public NuGetReplacer(IStringReplacer stringReplacer,
                             IFileFinder fileFinder,
                             IFileIO fileIO,
                             ISettings settings,
                             ITFSCheckout tFSCheckout)
        {
            _stringReplacer = stringReplacer;
            _FileFinder = fileFinder;
            _FileIO = fileIO;
            _Settings = settings;
            _TFSCheckout = tFSCheckout;
        }
        public void RepaceInFiles(string directory, string package, string version, string assemblyVersion,
                                  string csProjTargetFramework, string packageConfigTargetFramework, string publicKeyToken)
        {
            var filePatterns = new List<string> { ".csproj$", "packages.config$", "web.config$", "app.config$" };
            var filesList = _FileFinder.Find(directory, string.Join("|", filePatterns));

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
            var changedFiles = new HashSet<string>();
            foreach (var pattern in filePatterns)
            {
                if (!patternDictionary.TryGetValue(pattern, out List<Replacement> patterns))
                    continue;
                foreach (var file in filesList.Where(f => Regex.IsMatch(f, pattern, RegexOptions.IgnoreCase)))
                {
                    var fileContent = _FileIO.ReadAllText(file);
                    if (_stringReplacer.ReplaceInString(patterns, ref fileContent))
                    {
                        if (!changedFiles.Contains(file))
                            changedFiles.Add(file);
                        if (_Settings.DoNothing)
                            continue;
                        _FileIO.ClearReadonly(file);
                        _FileIO.WriteAllText(file, fileContent);
                    }
                }
            }

            if (_Settings.CheckoutFromTFS)
            {
                _TFSCheckout.Checkout(changedFiles);
            }

            Console.WriteLine($"Total Files Changed: {changedFiles.Count}");
            if (_Settings.DoNothing && changedFiles.Count > 0)
            {
                foreach (var file in changedFiles) 
                {
                    Console.WriteLine(file);
                }
            }
        }
    }
    public class PatternFilesMap
    {
        public string Patterns { get; set; }
        public HashSet<string> Files { get; set; }
    }
}
