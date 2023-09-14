# NuGetPackageUpdater
A tool the updates a NuGet package without reinstalling it, but by editing the version in the csproj and the packages.config.
This tool is most useful in a monolith or mono repo with 100s of projects.

## Download and Build NuGetPackageUpdater
1. Checkout the repo. This is a GitHub repo and can be checked out from here: https://github.com/rhyous/NuGetPackageUpdater
2. Open the src\NuGetPackageUpdater.sln file in Visual Studio.
3. Build the solution

## Running NuGetPackageUpdater
In Visual Studio with the NuGetPackageUpdater solution open . . .

This example assumes you have a package called YourPackage at version 2.1.0.

1. Right-click on the NuGetPackageUpdater project and choose properties.
2. Go to Debug | Open debug launch profiles UI
3. Enter the following in command line arguments
```
d="C:\path\to\your\repo" p=YourPackage v=2.1.0 ctf=net48 ed="C:\path\to\your\repo\some\folder\to\exclude" tfc=true dn=false
```

Note: tfc=true is what checks out from TFS. If you aren't using TFS, then lucky you. :-)

Note: dn=true means Do Nothing. It will simply list the files that will change but not change them. However, tfc=true will override it and will check out the files even if dn=true. However, the files will be changed.

## AssemblyVersion
If an additive or breaking change must be made, the assembly version should change. For example, imagine that we are going to Assembly Version 3.0.0 amd NuGet package version 3.0.1. You will need to enter the assembly version with the “a” parameter.

```
d="C:\path\to\your\repo" p=YourPackage a=3.0.0 v=3.0.1 ctf=net48 ed="C:\path\to\your\repo\some\folder\to\exclude" tfc=true dn=false
```

## Benefits
1. This will update all files in seconds.
2. Using for other NuGet Packages Yes, this updater can also be used for most other NuGet packages, but not all. 
Some NuGet packages, such as Newtonsoft. Newtonsoft also update the assemblyRedirect section of the App.config and this tool will also do that update. However, any NuGet package that adds custom lines to the top or bottom of a csproj file are not supported, such AWS. However, two things:
   1. This could be easily added to this tool.
   2. A combination of using this tool and using another tool, such as VSCode, to Find and Replace the lines (usually just the version) all csproj files with custom lines at the top and bottom these lines 
