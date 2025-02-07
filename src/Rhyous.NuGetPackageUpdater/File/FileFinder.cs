using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rhyous.NuGetPackageUpdater
{
    internal class FileFinder : IFileFinder
    {
        private readonly ISettings _Settings;

        public FileFinder(ISettings settings)
        {
            _Settings = settings;
        }

        public IList<string> Find(string directory, string pattern)
        {
            if (_Settings.ExcludeDirs != null && _Settings.ExcludeDirs.Contains(directory))
                return new List<string>();
            var fileList = new List<string>();
            var dirList = new List<string>();
            try
            {
                fileList.AddRange(Directory.GetFiles(directory).Where(f => Regex.IsMatch(f, pattern, RegexOptions.IgnoreCase)));
            }
            catch (Exception) { throw; }
            try { dirList.AddRange(Directory.GetDirectories(directory)); }
            catch (Exception) { throw; }
            foreach (var dir in dirList)
            {
                fileList.AddRange(Find(dir, pattern));
            }
            return fileList;
        }
    }
}
