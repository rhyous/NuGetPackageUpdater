using Rhyous.NuGetPackageUpdater;
using Rhyous.NuGetPackageUpdater.Models;
using Rhyous.StringLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhyous.NuGetPackageUpdater.Replacers
{


    /// <summary>
    /// 
    /// </summary>
    internal class CsprojReferenceWithNuGetReplacer : ICsprojReferenceWithNuGetReplacer
    {
        private readonly IFileFinder _FileFinder;
        private readonly IFileIO _FileIO;
        private readonly IPackageConfigInserter _PackageConfigInserter;
        private readonly ISettings _Settings;

        public CsprojReferenceWithNuGetReplacer(IFileFinder fileFinder,
                                                IFileIO fileIO,
                                                IPackageConfigInserter packageConfigInserter,
                                                ISettings settings)
        {
            _FileFinder = fileFinder;
            _FileIO = fileIO;
            _PackageConfigInserter = packageConfigInserter;
            _Settings = settings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="csproj"></param>
        /// <param name=""></param>
        public async Task ReplaceInFilesAsync(string directory, string csprojReferenceToReplace, string replacementSnippetFile, string packagesConfigSnippetFile)
        {
            var filePatterns = new List<string> { ".csproj$" };
            var csprojFiles = _FileFinder.Find(directory, string.Join("|", filePatterns));
            if (csprojFiles == null || !csprojFiles.Any())
                return;
            int csprojCount = 0;
            var replacementLines = await _FileIO.ReadAllLinesAsync(replacementSnippetFile);
            foreach (var csprojFile in csprojFiles)
            {
                if (ReplaceInFileAsync(csprojFile, csprojReferenceToReplace, replacementLines))
                {
                    csprojCount++;
                    var packageConfigPath = Path.Combine(Path.GetDirectoryName(csprojFile), "packages.config");
                    if (!_Settings.DoNothing)
                        _PackageConfigInserter.Insert(packageConfigPath, packagesConfigSnippetFile);
                }
            }
            Console.WriteLine($"Total Files: {csprojCount}");

        }

        internal bool ReplaceInFileAsync(string csprojFile, string csprojReferenceToReplace, string[] replacementLines)
        {
            var lines = _FileIO.ReadAllLines(csprojFile);
            var newLines = new List<string>();
            bool found = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (!found && lines[i].Contains(csprojReferenceToReplace, StringComparison.CurrentCultureIgnoreCase))
                {
                    found = true;
                    Console.WriteLine(csprojFile);
                    if (_Settings.DoNothing)
                        return found;
                    newLines.AddRange(replacementLines);

                    while (!lines[i].Contains("</ProjectReference>"))
                        i++;
                    i++; // One last i++ to coninue on the next line
                }
                newLines.Add(lines[i]);
            }
            if (found)
            {
                _FileIO.ClearReadonly(csprojFile);
                _FileIO.WriteAllLines(csprojFile, newLines.ToArray());
            }
            return found;
        }
    }
}

/* Use NuGet powershell command to replaces a snippet in a csproj like this . . .
   
   <ProjectReference Include="..\..\SomeLibrary\Some.Project\Some.Project.csproj">
     <Project>{4e9c1864-0fa4-4ea1-b75d-34c38cd079a1}</Project>
     <Name>Some.Project</Name>
   </ProjectReference> 

   . . . with a package reference like this (notice that the replacement could include a dependency


    <Reference Include="Some.Project, Version=2.0.1.0, Culture=neutral, processorArchitecture=MSIL">
      <HintPath>.\Packages\Some.Project.2.0.1\lib\netstandard2.0\Some.Project.dll</HintPath>
    </Reference>
    <Reference Include="Some.Dependency, Version=3.3.6.0, Culture=neutral, PublicKeyToken=c7192dc5380945e7, processorArchitecture=MSIL">
      <HintPath>.nuget\Packages\Some.Dependency.3.3.6\lib\net45\Some.Dependency.dll</HintPath>
    </Reference>

   . . . then if the packages.config doesn't exist, it should create it.
   . . . then adds a second snippet in the packages.config like this:

    <package id="Some.Project" version="2.0.1" targetFramework="net48" />
    <package id="Some.Dependency" version="3.3.6" targetFramework="net48" />
*/