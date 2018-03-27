using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhyous.NuGetPackageUpdater;

namespace Rhyous.NugetPacakgeUpdater.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestProjRegex()
        {
            // Arrange
            var package = "Rhyous.Odata";
            var version = "1.0.13";
            var fileContent = @"  <HintPath>..\..\packages\Rhyous.Odata.1.0.12\lib\net461\Rhyous.Odata.dll</HintPath> ";
            var expected = @"  <HintPath>..\..\packages\Rhyous.Odata.1.0.13\lib\net461\Rhyous.Odata.dll</HintPath> ";
            var replacements = new[] { CommonReplacements.GetHintPath(package, version) };

            // Act
            var result = Program.ReplaceInString(replacements, ref fileContent);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(expected, fileContent);
        }

        [TestMethod]
        public void TestProjRegex2()
        {
            // Arrange
            var package = "Rhyous.Odata";
            var version = "1.0.13";
            var fileContent = @"      <HintPath>..\..\packages\Rhyous.Odata.1.0.8\lib\net461\Rhyous.Odata.dll</HintPath>";
            var expected = @"      <HintPath>..\..\packages\Rhyous.Odata.1.0.13\lib\net461\Rhyous.Odata.dll</HintPath>";
            var replacements = new[] { CommonReplacements.GetHintPath(package, version) };

            // Act
            var result = Program.ReplaceInString(replacements, ref fileContent);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(expected, fileContent);
        }

        [TestMethod]
        public void TestProjRegexMultipleLines()
        {
            // Arrange
            var package = "Rhyous.Odata";
            var version = "1.0.13";
            var fileContent = "stuff then\r\n   <HintPath>..\\..\\packages\\Rhyous.Odata.1.0.12\\lib\\net461\\Rhyous.Odata.dll</HintPath>  \r\nstuff then more stuff";
            var expected = "stuff then\r\n   <HintPath>..\\..\\packages\\Rhyous.Odata.1.0.13\\lib\\net461\\Rhyous.Odata.dll</HintPath>  \r\nstuff then more stuff";
            var replacements = new[] { CommonReplacements.GetHintPath(package, version) };

            // Act
            var result = Program.ReplaceInString(replacements, ref fileContent);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(expected, fileContent);
        }

        [TestMethod]
        public void TestPackageConfigRegex()
        {
            // Arrange
            var package = "Rhyous.Odata";
            var version = "1.0.13";
            var fileContent = $"  <package id=\"Rhyous.Odata\" version=\"1.0.12\" targetFramework=\"net461\" />";
           var expected = $"  <package id=\"Rhyous.Odata\" version=\"1.0.13\" targetFramework=\"net461\" />";
            var replacements = new[] { CommonReplacements.GetPackagesConfig(package, version) };
            // Act
            var result = Program.ReplaceInString(replacements, ref fileContent);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(expected, fileContent);
        }

        [TestMethod]
        public void TestPackageConfigRegexMultipleLines()
        {
            // Arrange
            var package = "Rhyous.Odata";
            var version = "1.0.13";
            var fileContent = $"stuff then\r\n   <package id=\"Rhyous.Odata\" version=\"1.0.12\" targetFramework=\"net461\" />  \r\nstuff then more stuff";
            var expected = $"stuff then\r\n   <package id=\"Rhyous.Odata\" version=\"1.0.13\" targetFramework=\"net461\" />  \r\nstuff then more stuff";
            var replacements = new[] { CommonReplacements.GetPackagesConfig(package, version) };

            // Act
            var result = Program.ReplaceInString(replacements, ref fileContent);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(expected, fileContent);
        }

        [TestMethod]
        public void TestAssemblyAndHintPathLines()
        {
            // Arrange
            var package = "Rhyous.SimplePluginLoader";
            var version = "1.3.0";
            var assemblyVersion = "1.3.0.0";

            var fileContent = "    <Reference Include=\"Rhyous.SimplePluginLoader, Version=1.2.3.0, Culture=neutral, processorArchitecture=MSIL\">"
                            + @"      <HintPath>..\..\packages\Rhyous.SimplePluginLoader.1.2.5\lib\net40-client\Rhyous.SimplePluginLoader.dll</HintPath>"
                            + "    </Reference>";
                        
            var expected = "    <Reference Include=\"Rhyous.SimplePluginLoader, Version=1.3.0.0, Culture=neutral, processorArchitecture=MSIL\">"
                         + @"      <HintPath>..\..\packages\Rhyous.SimplePluginLoader.1.3.0\lib\net40-client\Rhyous.SimplePluginLoader.dll</HintPath>"
                         + "    </Reference>";

            var replacements = new[]
            {
                CommonReplacements.GetHintPath(package, version),
                CommonReplacements.GetReferenceInclude(package, assemblyVersion)
            };

            // Act
            var result = Program.ReplaceInString(replacements, ref fileContent);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(expected, fileContent);
        }
    }
}
