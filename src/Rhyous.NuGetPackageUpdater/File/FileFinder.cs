using Rhyous.NuGetPackageUpdater.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rhyous.NuGetPackageUpdater
{
    internal class FileFinder : IFileFinder
    {
        private readonly ISettings _Settings;
        private readonly IDirectoryWrapper _DirectoryWrapper;

        public FileFinder(ISettings settings, IDirectoryWrapper directoryWrapper)
        {
            _Settings = settings;
            _DirectoryWrapper = directoryWrapper;
        }

        public IList<string> Find(string directory, string pattern)
        {
            if (_Settings.ExcludeDirs != null && _Settings.ExcludeDirs.Contains(directory))
                return new List<string>();

            var fileList = new List<string>();
            var dirList = new List<string>();

            try
            {
                var files = _DirectoryWrapper.GetFiles(directory);
                fileList.AddRange(files.Where(f => Regex.IsMatch(f, pattern, RegexOptions.IgnoreCase)));
            }
            catch (Exception) 
            {
                throw;
            }

            try 
            { 
                var dirs = _DirectoryWrapper.GetDirectories(directory);
                dirList.AddRange(dirs); 
            }
            catch (Exception) 
            { 
                throw; 
            }

            foreach (var dir in dirList)
            {
                fileList.AddRange(Find(dir, pattern));
            }

            return fileList;
        }
    }
}
