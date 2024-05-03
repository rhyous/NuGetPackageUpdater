using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.NuGetPackageUpdater;
using Rhyous.NuGetPackageUpdater.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhyous.NugetPacakgeUpdater.Tests.File
{
    [TestClass]
    public class FileFinderTests
    {
        private MockRepository mockRepository;
        private Mock<ISettings> mockSettings;
        private Mock<IDirectoryWrapper> mockDirectoryWrapper;

        /// <summary>
        /// Helper function to get total count of items in a test dictionary.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private int GetFileCount(Dictionary<string, string[]> input)
        {
            int totalFiles = 0;

            foreach (var kvp in input)
            {
                totalFiles += kvp.Value.Length;
            }

            return totalFiles;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockSettings = mockRepository.Create<ISettings>();
            mockDirectoryWrapper = mockRepository.Create<IDirectoryWrapper>();
        }

        private FileFinder CreateFileFinder()
        {
            return new FileFinder(
                mockSettings.Object, mockDirectoryWrapper.Object);
        }

        [TestMethod]
        public void GetTotalFiles_FiveFilesInTwoKeyValuePairs_GetsFiveFiles()
        {
            //Arrange
            var testInput = new Dictionary<string, string[]>
            {
                {"first", new string[] {"one", "two", "three"} },
                {"second", new string[] {"four", "five"} }
            };

            //Act
            var actual = GetFileCount(testInput);

            //Assert
            Assert.AreEqual(5, actual);
        }
        
        [TestMethod]
        public void Find_ExcludeDirsIsNull_NoException()
        {
            // Arrange
            mockSettings.Setup(x => x.ExcludeDirs).Returns(new Func<List<string>>(() => null));
            mockDirectoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(Array.Empty<string>());
            mockDirectoryWrapper.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(Array.Empty<string>());

            var fileFinder = CreateFileFinder();
            string directory = @"C:\";
            string pattern = "";

            // Act
            var result = fileFinder.Find(directory, pattern);

            // Assert
            Assert.IsNotNull(result, "Don't throw an exception if Settings.ExcludeDirs is null.");
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Find_NoFilesOrDiretoriesFound_ReturnsEmptyList()
        {
            // Arrange
            mockSettings.Setup(x => x.ExcludeDirs).Returns(new List<string>());
            mockDirectoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(Array.Empty<string>());
            mockDirectoryWrapper.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(Array.Empty<string>());

            var fileFinder = CreateFileFinder();
            string directory = @"C:\";
            string pattern = "";

            // Act
            var result = fileFinder.Find(directory, pattern);

            // Assert
            Assert.AreEqual(0, result.Count, "Don't add false finds.");
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Find_ExcludeDirsContainsGivenPath_ReturnEmptyList()
        {
            //Arrange
            mockSettings.Setup(x => x.ExcludeDirs).Returns(new List<string> {@"C:\"});
            var subject = CreateFileFinder();
            string directory = @"C:\";
            string pattern = "";

            //Act
            var actual = subject.Find(directory, pattern);

            //Assert
            Assert.AreEqual(0, actual.Count, "Return an empty list if ExcludeDirs contains the current path.");
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Find_OneLevelThreeFiles_ReturnsThreeFiles()
        {
            //Arrange
            mockSettings.Setup(x => x.ExcludeDirs).Returns(new List<string>());
            var testFileList = new string[] {@"C:\one.txt", @"C:\two.txt", @"C:\three.txt"};

            mockDirectoryWrapper.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(Array.Empty<string>);
            mockDirectoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(testFileList);

            var subject = CreateFileFinder();
            string directory = @"C:\";
            string pattern = "";

            //Act
            var actual = subject.Find(directory, pattern);

            //Assert
            Assert.AreEqual(testFileList.Length, actual.Count);
            mockRepository.VerifyAll();
        }
        
        [TestMethod]
        public void Find_TwoLevelsFourFilesTotal_ReturnsFourFiles()
        {
            //Arrange
            mockSettings.Setup(x => x.ExcludeDirs).Returns(new List<string>());

            var mockFileStructure = new Dictionary<string, string[]> 
            {
                { @"C:\", new string[] { @"C:\one.txt", @"C:\two.txt" } },
                { @"C:\folder", new string[] { @"C:\folder\three.txt", @"C:\folder\four.txt" }}
            };
            var expectedFileCount = GetFileCount(mockFileStructure);
            
            var mockFolderStructure = new Dictionary<string, string[]>
            {
                { @"C:\", new string[] {@"C:\folder"} },
                { @"C:\folder" , Array.Empty<string>() }
            };

            mockDirectoryWrapper.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new Func<string, string[]>(path => mockFolderStructure[path]));
            mockDirectoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(new Func<string, string[]>(path => mockFileStructure[path]));

            var subject = CreateFileFinder();
            string directory = @"C:\";
            string pattern = "";

            //Act
            var actual = subject.Find(directory, pattern);

            //Assert
            Assert.AreEqual(expectedFileCount, actual.Count);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Find_ComplexStructure_GetsCorrectCountOfFiles()
        {
            //Arrange
            mockSettings.Setup(x => x.ExcludeDirs).Returns(new List<string>());

            var defaultDirectories = new Dictionary<string, string[]> 
            {
                { @"C:\",  new string[] {@"C:\bothInside", @"C:\onlyNested", @"C:\onlyFiles", @"C:\empty"} },
                { @"C:\bothInside", new string[] {@"C:\bothInside\firstNested"} },
                { @"C:\bothInside\firstNested", Array.Empty<string>() },
                { @"C:\onlyNested", new string[]{ @"C:\onlyNested\secondNested" } },
                { @"C:\onlyNested\secondNested", Array.Empty<string>() },
                { @"C:\onlyFiles", Array.Empty<string>() },
                { @"C:\empty", Array.Empty<string>() }
            };

            var defaultFiles = new Dictionary<string, string[]>()
            {
                { @"C:\",  Array.Empty<string>() },
                { @"C:\bothInside", new string[] {@"C:\bothInside\testing.txt"} },
                { @"C:\bothInside\firstNested", Array.Empty<string>() },
                { @"C:\onlyNested", Array.Empty<string>() },
                { @"C:\onlyNested\secondNested", Array.Empty<string>() },
                { @"C:\onlyFiles", new string[] { @"C:\onlyFiles\firstFile.txt", @"C:\onlyFiles\secondFile.txt" } },
                { @"C:\empty", Array.Empty<string>() }
            };

            var expectedFileCount = GetFileCount(defaultFiles);
            
            mockDirectoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(new Func<string, string[]>(path => defaultFiles[path]));
            mockDirectoryWrapper.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new Func<string, string[]>(path => defaultDirectories[path]));

            var subject = CreateFileFinder();

            //Act
            var actual = subject.Find(@"C:\", "");

            //Assert
            Assert.AreEqual(expectedFileCount, actual.Count);
        }

        [TestMethod]
        public void Find_ExcludeDirsHasExclusions_FindNonExcludedFiles()
        {
            //Arrange
            var dirs = new Dictionary<string, string[]>
            {
                { @"C:\", new string[] {@"C:\include", @"C:\exclude"} },
                { @"C:\include", new string[] {@"C:\include\exclude" } },
                { @"C:\include\exclude", Array.Empty<string>() },
                { @"C:\exclude", Array.Empty<string>() }
            };

            var files = new Dictionary<string, string[]>
            {
                { @"C:\", new string[] { @"C:\one.txt" } },
                { @"C:\include", new string[] { @"C:\include\two.txt" } },
                { @"C:\include\exclude", new string[] { @"C:\include\exclude\three.txt" } },
                { @"C:\exclude", new string[] { @"C:\exclude\four.txt" } }
            };

            var dirsToExclude = new List<string> {@"C:\exclude", @"C:\include\exclude"};

            mockDirectoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(new Func<string, string[]>(path => files[path]));
            mockDirectoryWrapper.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new Func<string, string[]>(path => dirs[path]));

            mockSettings.Setup(x => x.ExcludeDirs).Returns(dirsToExclude);

            var subject = CreateFileFinder();

            //Act
            var actual = subject.Find(@"C:\", "");

            //Assert
            Assert.AreEqual(2, actual.Count);
        }

        [TestMethod]
        public void Find_WithPattern_FindsCorrectFiles()
        {
            //Arrange
            var dirs = new Dictionary<string, string[]>
            {
                { @"C:\", Array.Empty<string>() }
            };

            var files = new Dictionary<string, string[]>
            {
                { @"C:\", new string[] { @"C:\correct.txt", @"C:\incorrect.xml" } }
            };

            mockDirectoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(new Func<string, string[]>(path => files[path]));
            mockDirectoryWrapper.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new Func<string, string[]>(path => dirs[path]));

            mockSettings.Setup(x => x.ExcludeDirs).Returns(new List<string>());

            var subject = CreateFileFinder();

            //Act
            var actual = subject.Find(@"C:\", ".txt");

            //Assert
            Assert.AreEqual(1, actual.Count);
            Assert.IsFalse(actual.Any(x => x.Contains(".xml")));
        }

        [TestMethod]
        public void Find_UnixFilePaths_FindsFiles()
        {
            //Arrange
            var dirs = new Dictionary<string, string[]>
            {
                { "/etc", Array.Empty<string>() }
            };

            var files = new Dictionary<string, string[]>
            {
                { "/etc", new string[] { "etc/correct.txt", "etc/incorrect.xml" } }
            };

            mockDirectoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(new Func<string, string[]>(path => files[path]));
            mockDirectoryWrapper.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new Func<string, string[]>(path => dirs[path]));

            mockSettings.Setup(x => x.ExcludeDirs).Returns(new List<string>());

            var subject = CreateFileFinder();

            //Act
            var actual = subject.Find("/etc", ".txt");

            //Assert
            Assert.AreEqual(1, actual.Count);
            Assert.IsFalse(actual.Any(x => x.Contains(".xml")));
        }

        
        [TestMethod]
        public void Find_PatternsAndExclusions_FindNonExcludedFiles()
        {
            //Arrange
            var dirs = new Dictionary<string, string[]>
            {
                { @"C:\", new string[] {@"C:\include", @"C:\exclude"} },
                { @"C:\include", new string[] {@"C:\include\exclude" } },
                { @"C:\include\exclude", Array.Empty<string>() },
                { @"C:\exclude", Array.Empty<string>() }
            };

            var files = new Dictionary<string, string[]>
            {
                { @"C:\", new string[] { @"C:\one.xml" } },
                { @"C:\include", new string[] { @"C:\include\two.txt", @"C:\include\failsPattern.xml" } },
                { @"C:\include\exclude", new string[] { @"C:\include\exclude\three.txt", @"C:\include\exclude\shouldNotGetFound.txt" } },
                { @"C:\exclude", new string[] { @"C:\exclude\four.txt" } }
            };

            var dirsToExclude = new List<string> {@"C:\exclude", @"C:\include\exclude"};

            mockDirectoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(new Func<string, string[]>(path => files[path]));
            mockDirectoryWrapper.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new Func<string, string[]>(path => dirs[path]));

            mockSettings.Setup(x => x.ExcludeDirs).Returns(dirsToExclude);

            var subject = CreateFileFinder();

            //Act
            var actual = subject.Find(@"C:\", ".txt");

            //Assert
            Assert.AreEqual(1, actual.Count);
        }
    }
}
