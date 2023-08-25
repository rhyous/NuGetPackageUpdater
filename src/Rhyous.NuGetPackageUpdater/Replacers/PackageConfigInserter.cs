using Rhyous.NuGetPackageUpdater;
using System.Linq;

namespace Rhyous.NuGetPackageUpdater.Replacers
{
    internal class PackageConfigInserter : IPackageConfigInserter
    {
        private readonly IFileIO _FileIO;

        public PackageConfigInserter(IFileIO fileIO)

        {
            _FileIO = fileIO;
        }
        public void Insert(string packagesConfigPath, string snippetFile)
        {
            var snippetLines = _FileIO.ReadAllLines(snippetFile);
            var startLines = new[] { "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                             "<packages>" };
            var endline = "</packages>";
            var existingPackageLines = snippetLines.ToList();
            if (_FileIO.Exists(packagesConfigPath))
            {
                foreach (var line in _FileIO.ReadLines(packagesConfigPath))
                {
                    if (line.Contains("<package "))
                        existingPackageLines.Add(line);
                }
                existingPackageLines.Sort();
                _FileIO.ClearReadonly(packagesConfigPath);
            }
            _FileIO.WriteAllLines(packagesConfigPath, startLines);
            _FileIO.AppendAllLines(packagesConfigPath, snippetLines);
            _FileIO.AppendLine(packagesConfigPath, endline);
        }
    }
}