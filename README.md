[![Build status](https://ci.appveyor.com/api/projects/status/4ap2v3ybhac9c3lh/branch/master?svg=true)](https://ci.appveyor.com/project/luau-project/luainstaller/branch/master)

# Overview

The toolset available here aids the process to download, build and install Lua versions directly from http://www.lua.org , either manually or programmatically.

In the current stage, there are three projects:

1. **LuaInstaller.Core**

    Includes the main functionality to install Lua completely on your computer and the other projects are
    built around this core project as an interface to the enduser

2. **LuaInstaller.Console**

    This is a command line program meant to be used by CI servers like AppVeyor

3. **LuaInstaller**

    An application aimed to endusers, delivering a Windows Presentation Foundation (WPF) Graphical User Interface (GUI)
    offering the core functionality to seamlessly download, build and install on the target machine

# Setup

Before any interaction with the tools provided here, the user **MUST HAVE** Visual Studio with support for WIN32 C++ development **OR** Visual C++ Build Tools (only the build tools, not the Visual Studio IDE) installed. Upon installation, these tools automatically installs the Windows SDK, also required by the tools provided here.

Furthermore, the user **MUST HAVE** .NET Framework 4.0 or newer installed to run the tools, which comes bundled by default in the latest versions of Windows (7, 8, etc).

# Future Work

We want to ease the process to have latest verions of Lua and LuaRocks installed in the system, so LuaRocks support is on top of our todo list.