﻿using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhyous.NuGetPackageUpdater;

namespace Rhyous.NugetPacakgeUpdater.Tests
{
    [TestClass]
    public class TestReplacements
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
        public void TestPackageConfig_TargetFramework_Regex()
        {
            // Arrange
            var package = "Rhyous.Odata";
            var version = "1.0.13";
            var fileContent = $"  <package id=\"Rhyous.Odata\" version=\"1.0.12\" targetFramework=\"net46\" />";
            var expected = $"  <package id=\"Rhyous.Odata\" version=\"1.0.13\" targetFramework=\"net46\" />";
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

        [TestMethod]
        public void TestGetProjectReference()
        {
            // Arrange
            var package = "Rhyous.Odata";
            var version = "1.1.3";

            var fileContent = "  <ItemGroup>"
                            + "    <PackageReference Include=\"Rhyous.Odata\" Version=\"1.1.2\" />"
                            + "    <PackageReference Include=\"System.ComponentModel.Annotations\" Version=\"4.6.0\" />"
                            + "  </ItemGroup>";

            var expected = "  <ItemGroup>"
                         + "    <PackageReference Include=\"Rhyous.Odata\" Version=\"1.1.3\" />"
                         + "    <PackageReference Include=\"System.ComponentModel.Annotations\" Version=\"4.6.0\" />"
                         + "  </ItemGroup>";

            var replacements = new[]
            {
                CommonReplacements.GetProjectReference(package, version)
            };

            // Act
            var result = Program.ReplaceInString(replacements, ref fileContent);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(expected, fileContent);
        }

        [TestMethod]
        public void TestGetProjectReferenceNotClosed()
        {
            // Arrange
            var package = "Rhyous.Odata";
            var version = "1.1.3";

            var fileContent = "  <ItemGroup>"
                            + "    <PackageReference Include=\"Rhyous.Odata\" Version=\"1.1.2\" />"
                            + "    <PackageReference Include=\"System.ComponentModel.Annotations\" Version=\"4.6.0\" >"
                            + "  </ItemGroup>";

            var expected = "  <ItemGroup>"
                         + "    <PackageReference Include=\"Rhyous.Odata\" Version=\"1.1.3\" />"
                         + "    <PackageReference Include=\"System.ComponentModel.Annotations\" Version=\"4.6.0\" >"
                         + "  </ItemGroup>";

            var replacements = new[]
            {
                CommonReplacements.GetProjectReference(package, version)
            };

            // Act
            var result = Program.ReplaceInString(replacements, ref fileContent);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(expected, fileContent);
        }


        [TestMethod]
        public void TestAppConfigReplacement()
        {
            // Arrange
            var package = "Newtonsoft.Json";
            var assemblyVersion = "11.0.0.0";

            var fileContent = "      <dependentAssembly>"
                            + "        <assemblyIdentity name=\"Newtonsoft.Json\" publicKeyToken=\"30ad4fe6b2a6aeed\" culture=\"neutral\" />"
                            + "        <bindingRedirect oldVersion=\"0.0.0.0-10.0.0.0\" newVersion=\"10.0.0.0\" />"
                            + "      </dependentAssembly>";

            var expected = "      <dependentAssembly>"
                         + "        <assemblyIdentity name=\"Newtonsoft.Json\" publicKeyToken=\"30ad4fe6b2a6aeed\" culture=\"neutral\" />"
                         + "        <bindingRedirect oldVersion=\"0.0.0.0-11.0.0.0\" newVersion=\"11.0.0.0\" />"
                         + "      </dependentAssembly>";

            var replacements = new[]
            {
                CommonReplacements.GetWebConfig(package, assemblyVersion),
            };

            // Act
            var result = Program.ReplaceInString(replacements, ref fileContent);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(expected, fileContent);
        }

        [TestMethod]
        public void TestAppConfigReplacementFullAppConfig()
        {
            // Arrange
            var package = "Newtonsoft.Json";
            var assemblyVersion = "11.0.0.0";

            var fileContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                            + "<configuration>"
                            + "  <configSections>"
                            + "    "
                            + "    <section name=\"entityFramework\" type=\"System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" requirePermission=\"false\" />"
                            + "  <!-- For more information on Entity Framework configuration, visit http://go.microsoft.com/fwlink/?LinkID=237468 --></configSections>"
                            + "  <entityFramework>"
                            + "    <defaultConnectionFactory type=\"System.Data.Entity.Infrastructure.LocalDbConnectionFactory, EntityFramework\">"
                            + "      <parameters>"
                            + "        <parameter value=\"mssqllocaldb\" />"
                            + "      </parameters>"
                            + "    </defaultConnectionFactory>"
                            + "    <providers>"
                            + "      <provider invariantName=\"System.Data.SqlClient\" type=\"System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer\" />"
                            + "    </providers>"
                            + "  </entityFramework>"
                            + "  <runtime>"
                            + "    <assemblyBinding xmlns=\"urn:schemas-microsoft-com:asm.v1\">"
                            + "      <dependentAssembly>"
                            + "        <assemblyIdentity name=\"Newtonsoft.Json\" publicKeyToken=\"30ad4fe6b2a6aeed\" culture=\"neutral\" />"
                            + "        <bindingRedirect oldVersion=\"0.0.0.0-10.0.0.0\" newVersion=\"10.0.0.0\" />"
                            + "      </dependentAssembly>"
                            + "      <dependentAssembly>"
                            + "        <assemblyIdentity name=\"LinqKit.Core\" publicKeyToken=\"bc217f8844052a91\" culture=\"neutral\" />"
                            + "        <bindingRedirect oldVersion=\"0.0.0.0-1.1.15.0\" newVersion=\"1.1.15.0\" />"
                            + "      </dependentAssembly>"
                            + "    </assemblyBinding>"
                            + "  </runtime>"
                            + "</configuration>";

            var expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                            + "<configuration>"
                            + "  <configSections>"
                            + "    "
                            + "    <section name=\"entityFramework\" type=\"System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" requirePermission=\"false\" />"
                            + "  <!-- For more information on Entity Framework configuration, visit http://go.microsoft.com/fwlink/?LinkID=237468 --></configSections>"
                            + "  <entityFramework>"
                            + "    <defaultConnectionFactory type=\"System.Data.Entity.Infrastructure.LocalDbConnectionFactory, EntityFramework\">"
                            + "      <parameters>"
                            + "        <parameter value=\"mssqllocaldb\" />"
                            + "      </parameters>"
                            + "    </defaultConnectionFactory>"
                            + "    <providers>"
                            + "      <provider invariantName=\"System.Data.SqlClient\" type=\"System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer\" />"
                            + "    </providers>"
                            + "  </entityFramework>"
                            + "  <runtime>"
                            + "    <assemblyBinding xmlns=\"urn:schemas-microsoft-com:asm.v1\">"
                            + "      <dependentAssembly>"
                            + "        <assemblyIdentity name=\"Newtonsoft.Json\" publicKeyToken=\"30ad4fe6b2a6aeed\" culture=\"neutral\" />"
                            + "        <bindingRedirect oldVersion=\"0.0.0.0-11.0.0.0\" newVersion=\"11.0.0.0\" />"
                            + "      </dependentAssembly>"
                            + "      <dependentAssembly>"
                            + "        <assemblyIdentity name=\"LinqKit.Core\" publicKeyToken=\"bc217f8844052a91\" culture=\"neutral\" />"
                            + "        <bindingRedirect oldVersion=\"0.0.0.0-1.1.15.0\" newVersion=\"1.1.15.0\" />"
                            + "      </dependentAssembly>"
                            + "    </assemblyBinding>"
                            + "  </runtime>"
                            + "</configuration>";

            var replacements = new[]
            {
                CommonReplacements.GetWebConfig(package, assemblyVersion),
            };

            // Act
            var result = Program.ReplaceInString(replacements, ref fileContent);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(expected, fileContent);
        }
    }
}
