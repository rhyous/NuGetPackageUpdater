using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rhyous.NuGetPackageUpdater
{
    internal interface IFileIO
    {
        void ClearReadonly(string path);
        bool Exists(string path);
        string ReadAllText(string path);
        IEnumerable<string> ReadLines(string path);
        string[] ReadAllLines(string path);
        Task<string[]> ReadAllLinesAsync(string path);
        void WriteAllText(string path, string contents);
        void AppendLine(string path, string line);
        void AppendAllLines(string path, string[] lines);
        void WriteAllLines(string path, string[] lines);
    }
}