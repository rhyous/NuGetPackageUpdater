using Rhyous.SimpleArgs;
using System;
using System.Collections.Generic;

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
                    IsRequired = false
                },
                new Argument
                {
                    Name = "Package",
                    ShortName = "p",
                    Description = "The NuGet package to update.",
                    Example = "{name}=Rhyous.StringLibrary",
                    IsRequired = true
                },
                new Argument
                {
                    Name = "Version",
                    ShortName = "v",
                    Description = "The version of the NuGet package to update to.",
                    Example = "{name}=1.1.0",
                    IsRequired = true
                },
                new Argument
                {
                    Name = "AssemblyVersion",
                    ShortName = "A",
                    Description = "The version of the NuGet Package's dll assembly o update to.",
                    Example = "{name}=1.0.0.0",
                    IsRequired = false
                },
                new Argument
                {
                    Name = "AssemblyPublicKeyToken",
                    ShortName = "T",
                    Description = "The PublicKeyToken of the NuGet Package's dll assembly o update.",
                    Example = "{name}=1.0.0.0",
                    IsRequired = false
                }
            });
        }

        public override void HandleArgs(IReadArgs inArgsHandler)
        {
            base.HandleArgs(inArgsHandler);
            Program.OnArgumentsHandled();
        }
    }
}
