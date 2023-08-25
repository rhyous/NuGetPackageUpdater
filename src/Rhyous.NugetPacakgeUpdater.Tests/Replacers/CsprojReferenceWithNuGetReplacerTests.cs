using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.NuGetPackageUpdater;
using Rhyous.NuGetPackageUpdater.Replacers;
using Rhyous.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rhyous.NugetPacakgeUpdater.Tests.Replacers
{
    [TestClass]
    public class CsprojReferenceWithNuGetReplacerTests
    {
        private MockRepository _MockRepository;

        private Mock<IFileFinder> _MockFileFinder;
        private Mock<IFileIO> _MockFileIO;
        private Mock<IPackageConfigInserter> _MockPackageConfigInserter;
        private Mock<ISettings> _MockSettings;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockFileFinder = _MockRepository.Create<IFileFinder>();
            _MockFileIO = _MockRepository.Create<IFileIO>();
            _MockPackageConfigInserter = _MockRepository.Create<IPackageConfigInserter>();
            _MockSettings = _MockRepository.Create<ISettings>();
        }

        private CsprojReferenceWithNuGetReplacer CreateCsprojReferenceWithNuGetReplacer()
        {
            return new CsprojReferenceWithNuGetReplacer(
                _MockFileFinder.Object,
                _MockFileIO.Object,
                _MockPackageConfigInserter.Object,
                _MockSettings.Object);
        }

        #region ReplaceInFilesAsync
        [TestMethod]
        [ListTNullOrEmpty(typeof(string))]
        public async Task CsprojReferenceWithNuGetReplacer_ReplaceInFilesAsync_StateUnderTest_ExpectedBehavior(List<string> fileList)
        {
            // Arrange
            var csprojReferenceWithNuGetReplacer = CreateCsprojReferenceWithNuGetReplacer();
            string directory = null;
            string csprojReferenceToReplace = null;
            string replacementCsprojSnippetFile = null;
            string replacementPackagesConfigSnippetFile = null;
            _MockFileFinder.Setup(m => m.Find(null, ".csproj$")).Returns(fileList);

            // Act
            await csprojReferenceWithNuGetReplacer.ReplaceInFilesAsync(directory, csprojReferenceToReplace, replacementCsprojSnippetFile, replacementPackagesConfigSnippetFile);

            // Assert
            _MockRepository.VerifyAll();
        }
        #endregion

        #region ReplaceInFileAsync
        [TestMethod]
        public void CsprojReferenceWithNuGetReplacer_ReplaceInFileAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var csprojReferenceWithNuGetReplacer = CreateCsprojReferenceWithNuGetReplacer();
            string csproj = @"c:\some\path\to\Fake.csproj";
            string csprojReferenceToReplace = "Some.Project.csproj";
            var firstLine = " Some line (or multiple lines) before what is to be found.";
            var lastLine = " Some line (or multiple lines) after what is to be found.";
            string[] csprojLines = new[] { firstLine,
                                           $"   <ProjectReference Include=\"..\\..\\SomeLibrary\\Some.Project\\{csprojReferenceToReplace}\">",
                                            "     <Project>{4e9c1864-0fa4-4ea1-b75d-34c38cd079a1}</Project>",
                                           $"     <Name>{csprojReferenceToReplace}</Name>",
                                            "   </ProjectReference>",
                                            lastLine};

            string[] replacementLines = new[] { "Fake Line 0", "Fake Line 1" };
            _MockFileIO.Setup(m => m.ReadAllLines(csproj)).Returns(csprojLines);
            _MockFileIO.Setup(m => m.WriteAllLines(csproj,
                                                   It.Is<string[]>(s => s[0] == firstLine
                                                                     && s[1] == replacementLines[0]
                                                                     && s[2] == replacementLines[1]
                                                                     && s[3] == lastLine)));
            _MockSettings.Setup(m=>m.DoNothing).Returns(false);
            _MockFileIO.Setup(m => m.ClearReadonly(csproj));

            // Act
            bool actual = csprojReferenceWithNuGetReplacer.ReplaceInFileAsync(csproj, csprojReferenceToReplace, replacementLines);

            // Assert
            Assert.IsTrue(actual);
            _MockRepository.VerifyAll();
        }
        #endregion
    }
}
