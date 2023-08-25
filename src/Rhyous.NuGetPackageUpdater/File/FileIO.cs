using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rhyous.NuGetPackageUpdater
{
    internal class FileIO : IFileIO
    {
        public void ClearReadonly(string path) => new FileInfo(path).IsReadOnly = false;
        public bool Exists(string path) => File.Exists(path);
        public string ReadAllText(string path) => File.ReadAllText(path);
        public string[] ReadAllLines(string path) => File.ReadAllLines(path);
        public async Task<string[]> ReadAllLinesAsync(string path) => await File.ReadAllLinesAsync(path);
        public IEnumerable<string> ReadLines(string path) => File.ReadLines(path);
        public void AppendLine(string path, string line) => File.AppendAllLines(path, new[] { line });
        public void AppendAllLines(string path, string[] lines) => File.AppendAllLines(path, lines);
        public void WriteAllLines(string path, string[] lines) => File.WriteAllLines(path, lines);

        public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);
    }
}