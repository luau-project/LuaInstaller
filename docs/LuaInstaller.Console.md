# LuaInstaller.Console

## Overview

This tool is meant to be used from the command prompt ```cmd.exe``` or in CI servers by people that need to test Lua libraries on Windows. Thus, this tool frees the developer from the hassle to create scripts to perform Lua installation.

## Table of Contents

* [Get Informations](#get-informations)
    * [Display the help message](#display-the-help-message)
    * [Display the version of the installer](#display-the-version-of-the-installer)
    * [Display the website of the installer](#display-the-website-of-the-installer)
    * [List Lua versions installable by these tools](#list-lua-versions-installable-by-these-tools)
    * [List MSVC versions installed in the system for x86 compilation](#list-msvc-versions-installed-in-the-system-for-x86-compilation)
    * [List MSVC versions installed in the system for x64 compilation](#list-msvc-versions-installed-in-the-system-for-x64-compilation)
    * [List Windows SDK versions installed in the system for x86 compilation](#list-windows-sdk-versions-installed-in-the-system-for-x86-compilation)
    * [List Windows SDK versions installed in the system for x64 compilation](#list-windows-sdk-versions-installed-in-the-system-for-x64-compilation)
* [Installation](#installation)
    * [Install the latest Lua using the most recent tools](#install-the-latest-lua-using-the-most-recent-tools)
    * [Install a specific Lua version on a directory employing the most recent tools](#install-a-specific-lua-version-on-a-directory-employing-the-most-recent-tools)
    * [Machine-wide installation (Run As Admin - required)](#machine-wide-installation-run-as-admin---required)

## Get Informations

```txt
LuaInstaller.Console.exe [ /? | help ]
    Displays this help message

LuaInstaller.Console.exe [ -v | --version ]
    Displays the version of the installer

LuaInstaller.Console.exe help-website
    Displays the website of the installer

LuaInstaller.Console.exe list-lua
    Lists all Lua versions that this tool
    is able to build

LuaInstaller.Console.exe list-vs
    Lists all MSVC compilers found
    for x64 on 64 Bit Operating
    Systems or x86 otherwise.

LuaInstaller.Console.exe list-vs-x86
    Lists all MSVC x86 toolset
    compilers found

LuaInstaller.Console.exe list-vs-x64
    Lists all MSVC x64 toolset
    compilers found

LuaInstaller.Console.exe list-win-sdk
    Lists all Windows SDK found
    for x64 on 64 Bit Operating
    Systems or x86 otherwise

LuaInstaller.Console.exe list-win-sdk-x86
    Lists all Windows SDK x86 found

LuaInstaller.Console.exe list-win-sdk-x64
    Lists all Windows SDK x64 found
```

### Display the help message

```batch
LuaInstaller.Console.exe help
```

### Display the version of the installer

```batch
LuaInstaller.Console.exe --version
```

### Display the website of the installer

```batch
LuaInstaller.Console.exe help-website
```

### List Lua versions installable by these tools

```batch
LuaInstaller.Console.exe list-lua
```

### List MSVC versions installed in the system for x86 compilation

```batch
LuaInstaller.Console.exe list-vs-x86
```

### List MSVC versions installed in the system for x64 compilation

```batch
LuaInstaller.Console.exe list-vs-x64
```

### List Windows SDK versions installed in the system for x86 compilation

```batch
LuaInstaller.Console.exe list-win-sdk-x86
```

### List Windows SDK versions installed in the system for x64 compilation

```batch
LuaInstaller.Console.exe list-win-sdk-x64
```

## Installation

```txt
LuaInstaller.Console.exe install { OPTION=VALUE }

    dest-dir=C:\test
        Destination directory to install
        e.g.: installs Lua on C:\test
        Defaults to the current directory.
        
    version=5.1.5
        Lua version to install
        e.g.: installs Lua 5.1.5

    arch=[ x86 | X86 | x64 | X64 ]
        Generates machine code for the
        specified platform
        Defaults to x64 on 64 Bit Operating
        Systems or x86 otherwise.

    vs=14.0
        Uses a specific version of the
        Microsoft Visual Studio toolset
        to build Lua
        e.g.: Uses VS 14.0 (VS 2015)

    win-sdk=10.0
        Uses a specific version of the
        Windows SDK
        e.g.: Uses Win SDK for Windows 10

    env-var=[ process | user | machine ]
        Sets environment variables
        for the specified target.
        Remark: if this switch is not
        specified, environment variables
        are not set at all.
```

### Install the latest Lua using the most recent tools

```batch
LuaInstaller.Console.exe install
```

*Description*: Installs the latest version of Lua, using the latest version of MSVC and Windows SDK for x64 on a 64 Bit operating system or x86 architecture otherwise.

### Install a specific Lua version on a directory employing the most recent tools

```batch
LuaInstaller.Console.exe install "dest-dir=C:\Lua-5.2.4" version=5.2.4
```

*Description*: Installs Lua 5.2.4, using the latest version of MSVC and Windows SDK for x64 on a 64 Bit operating system or x86 architecture otherwise.

### Machine-wide installation (Run As Admin - required)

```batch
LuaInstaller.Console.exe install "dest-dir=C:\Program Files (x86)\Lua" version=5.1.5 env-var=machine
```

*Description*: Installs Lua ```5.1.5``` in the ```C:\Program Files (x86)\Lua``` folder and also sets environment variables
machine-wide.

[Back to the Docs](../docs/README.md)