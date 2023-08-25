using Rhyous.SimpleArgs;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rhyous.NuGetPackageUpdater
{
    // Add this line of code to Main() in Program.cs
    //
    //   new ArgsManager<ArgsHandler>().Start(args);
    //

    /// <summary>
    /// A class that implements IArgumentsHandler where command line
    /// arguments are defined.
    /// </summary>
    public sealed class ArgsHandler : ArgsHandlerBase
    {
        public override void InitializeArguments(IArgsManager argsManager)
        {
            Arguments.AddRange(new List<Argument>
            {
                new Argument
                {
                    Name = "Directory",
                    ShortName = "d",
                    Description = "The directory to find project files and configs. The default is the directory where the exe was launched.",
                    Example = "{name}=d:\\dev",
                    DefaultValue = AppDomain.CurrentDomain.BaseDirectory,
                    IsRequired = false,
                    CustomValidation = Directory.Exists
                },
                new Argument
                {
                    Name = "ExcludeDirectories",
                    ShortName = "ed",
                    Description = "Directories to skip, semicolon separated.",
                    Example = "{name}=c:\\dev\folderToSkip",                    
                    IsRequired = false
                },
                new Argument
                {
                    Name = "Package",
                    ShortName = "p",
                    Description = "The NuGet package to update.",
                    Example = "{name}=Rhyous.StringLibrary",
                    IsRequired = false
                },
                new Argument
                {
                    Name = "Version",
                    ShortName = "v",
                    Description = "The version of the NuGet package to update to.",
                    Example = "{name}=1.1.0",
                    IsRequired = false
                },
                new Argument
                {
                    Name = "AssemblyVersion",
                    ShortName = "A",
                    Description = "The version of the NuGet Package's dll assembly to update to.",
                    Example = "{name}=1.0.0.0",
                    IsRequired = false
                },
                new Argument
                {
                    Name = "AssemblyPublicKeyToken",
                    ShortName = "T",
                    Description = "The PublicKeyToken of the NuGet Package's dll assembly to update.",
                    Example = "{name}=1.0.0.0",
                    IsRequired = false
                },
                new Argument
                {
                    Name = "CsprojTargetFramework",
                    ShortName = "ctf",
                    Description = "The target framework, ie. net461, net48, netstandard2.0, etc.",
                    Example = "{name}=netstandard2.0",
                    IsRequired = false
                },
                new Argument
                {
                    Name = "PackagesConfigTargetFramework",
                    ShortName = "pctf",
                    Description = "The target framework, ie. net461, net48, netstandard2.0, etc.",
                    Example = "{name}=netstandard2.0",
                    IsRequired = false
                },
                new Argument
                {
                    Name = "ReplaceCsprojReference",
                    ShortName = "r",
                    Description = "Set this to true if the reference to replace is a reference to another csproj and not a NuGet package.",
                    Example = "{name}=true",
                    DefaultValue = false.ToString(),
                    AllowedValues = CommonAllowedValues.TrueFalse,
                    IsRequired = false
                },
                new Argument
                {
                    Name = "CsprojReferenceToReplace",
                    ShortName = "cref",
                    Description = "The csproj reference to replace.",
                    Example = "{name}=SomeProject.csproj",
                    IsRequired = false
                },
                new Argument
                {
                    Name = "CsprojReferenceReplacementSnippetFile",
                    ShortName = "crefsnip",
                    Description = "The file containing the snippet to replace in the csproj reference.",
                    Example = "{name}=Somesnippetfile.txt",
                    IsRequired = false,
                    CustomValidation = File.Exists,
                },
                new Argument
                {
                    Name = "PackagesConfigSnippetFile",
                    ShortName = "pcsnip",
                    Description = "The file containing the snippet to add to the packages.config.",
                    Example = "{name}=Somesnippetfile.txt",
                    IsRequired = false,
                    CustomValidation = File.Exists
                },
                new Argument
                {
                    Name = "DoNothing",
                    ShortName = "dn",
                    Description = "Instead of doing anything, it will just list what it would have changed.",
                    Example = "{name}=true",
                    IsRequired = false,
                    DefaultValue = "false"
                },
                new Argument
                {
                    Name = "TFSCheckout",
                    ShortName = "tfc",
                    Description = "Check out the files with TFS.",
                    Example = "{name}=true",
                    IsRequired = false,
                    DefaultValue = "false"
                },
                new Argument
                {
                    Name = "TFdotExePath",
                    ShortName = "tf",
                    Description = "Check out the files with TFS.",
                    Example = "{name}=true",
                    IsRequired = false,
                    DefaultValue = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Enterprise\\Common7\\IDE\\CommonExtensions\\Microsoft\\TeamFoundation\\Team Explorer\\TF.exe",
                    CustomValidation = File.Exists
                }
            });
        }

        public override void HandleArgs(IReadArgs inArgsHandler)
        {
            base.HandleArgs(inArgsHandler);
        }
    }
}
