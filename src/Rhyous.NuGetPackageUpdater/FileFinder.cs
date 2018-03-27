using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rhyous.NuGetPackageUpdater
{
    public class FileFinder
    {
        public static IEnumerable<string> Find(string directory, string pattern)
        {
            var fileList = new List<string>();
            var dirList = new List<string>();
            try
            {
                fileList.AddRange(Directory.GetFiles(directory).Where(f => Regex.IsMatch(f, pattern)));
            }
            catch (Exception e) { throw; }
            try { dirList.AddRange(Directory.GetDirectories(directory)); }
            catch (Exception e) { throw; }
            foreach (var dir in dirList)
            {
                fileList.AddRange(Find(dir, pattern));
            }
            return fileList;
        }
    }
}
