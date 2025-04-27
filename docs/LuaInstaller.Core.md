# LuaInstaller.Core

> [!NOTE]
> 
> The LuaInstaller project does not support cross-compilation. This means that you must build for the architecture of the underlying operating system (x64 for a 64-Bit Windows, x86 for a 32-bit Windows, and ARM64 for Windows on ARM).

## Overview

Includes the main functionality to install Lua completely on your computer. The other projects are built around this core project as an interface to the end-user. You can even embed it on your own .NET projects to have a Lua installer feature available.

## Table of Contents

* [Usage](#usage)
    * [Query Lua versions from the website](#query-lua-versions-from-the-website)
    * [Query the latest Lua version from the website](#query-the-latest-lua-version-from-the-website)
    * [Query all the Visual Studio instances with build tools found on your computer (x64)](#query-all-the-visual-studio-instances-with-build-tools-found-on-your-computer-x64)
    * [Query all the Windows SDKs found on your computer (x64)](#query-all-the-windows-sdks-found-on-your-computer-x64)
    * [Perform a download, build and install of Lua (x64)](#perform-a-download-build-and-install-of-lua-x64)

## Usage

### Query Lua versions from the website:

```cs
LuaVersion[] versions = LuaWebsite.QueryVersions();
foreach (LuaVersion v in versions)
{
    Console.WriteLine(" -> Lua " + v.Version);
}
```

### Query the latest Lua version from the website:

```cs
LuaVersion latest;
if (LuaWebsite.TryGetLatestVersion(out latest))
{
    Console.WriteLine("Latest Lua version is: " + latest.Version);
}
else
{
    Console.WriteLine("Unable to get the latest version. Please, check your internet connection and try again.");
}
```

### Query all the Visual Studio instances with build tools found on your computer (x64)

```cs
IInstalledComponents components = new InstalledComponents();

foreach (VisualStudio vs in components.AllVisualStudioX64())
{
    Console.WriteLine("Installation directory: " + vs.VsDir);
    Console.WriteLine("MSVC directory: " + vs.VcDir);
    Console.WriteLine(vs.Version.ToString());
}
```

### Query all the Windows SDKs found on your computer (x64)

```cs
IInstalledComponents components = new InstalledComponents();

foreach (WindowsSdk sdk in components.AllWindowsSdkX64())
{
    Console.WriteLine(sdk.Version.ToString());
}
```

### Perform a download, build and install of Lua (x64)

```cs
LuaVersion luaVersion = ...; /* query as above */
VisualStudio vs = ...; /* query some x64 Visual Studio as above */
WindowsSdk sdk = ...; /* query some x64 Windows SDK as above */

/*
    EnvironmentVariableTarget.User or EnvironmentVariableTarget.Machine
    requires Admin privileges
*/
EnvironmentVariableTarget? env = null; 

VisualStudioToolset toolset = vs.Toolset;

ICompiler compiler = new VisualStudioCompiler(toolset.Cl);
ILinker linker = new VisualStudioLinker(toolset.Link);

InstallationManager manager = new InstallationManager(compiler, linker);

manager.InstallationProgressChanged += (sender, e) =>
{
    if (e.Progress == Finished)
    {
        Console.WriteLine("Lua was installed!");
    }
};

// The next line will block the current thread.
// It is a good idea to run this operation
// on a background worker thread
// (System.ComponentModel.BackgroundWorker),
// and listen to progress changes.
manager.ExecuteInstall(luaVersion, "C:\\Lua", vs, sdk, env);
```

[Back to the Docs](../docs/README.md)