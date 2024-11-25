## LuaInstaller Documentation

Welcome to the LuaInstaller documentation!

## End-Users

Did you get here by a mistake? Back to the project's [home page](../).

## Developers

Are you looking for CI / CD content? [check our CI examples here](./UsageCI.md).

> [!IMPORTANT]
> 
> Before you try to use the tools and libraries provided here, be sure to perform the [initial setup](../README.md#setup).

In the current stage, there are three projects:

1. **LuaInstaller.Core**

    Includes the main functionality to install Lua completely on your computer. The other projects are built around this core project as an interface to the end-user. You can even embed it on your own .NET projects to have a Lua installer feature available. For more information, [read here](./LuaInstaller.Core.md#overview).

2. **LuaInstaller.Console**

    This is a command line program meant to be used by CI servers like AppVeyor, GitHub Actions and others. It can also benefit those people who like to work from the command line. For more information, [read here](./LuaInstaller.Console.md#overview).

3. **LuaInstaller**

    An application aimed to endusers, delivering a Windows Presentation Foundation (WPF) Graphical User Interface (GUI) offering the core functionality to seamlessly download, build and install Lua on the target Windows machine.