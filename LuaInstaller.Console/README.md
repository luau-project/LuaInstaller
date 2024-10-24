﻿# LuaInstaller.Console

## Overview

This tool is meant to be used from the command prompt ```cmd.exe``` or in CI servers by people that needs to test Lua libraries on Windows. Thus, this tool frees the developer from the hassle to create scripts to perform Lua installation.

## Get Informations

```
LuaInstaller.Console.exe [ /? | help ]
    Displays this help message

LuaInstaller.Console.exe list-lua
    Lists all Lua versions that this tool
    is able to build

LuaInstaller.Console.exe list-vs-x86
    Lists all MSVC x86 toolset
    compilers found

LuaInstaller.Console.exe list-vs-x64
    Lists all MSVC x64 toolset
    compilers found

LuaInstaller.Console.exe list-win-sdk
    Lists all Windows SDK found
```

### List Lua versions installable by these tools

```
LuaInstaller.Console.exe list-lua
```

### List Visual C++ versions installed in the system for x86 compilation

```
LuaInstaller.Console.exe list-vs-x86
```

### List Visual C++ versions installed in the system for x64 compilation

```
LuaInstaller.Console.exe list-vs-x64
```

## Installation

```
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

### Local installation

```
LuaInstaller.Console.exe install
```

> [!NOTE]
> 
> Installs the latest version of Lua, using the latest version of MSVC and Windows SDK for x64 on a 64 Bit operating system or x86 architecture otherwise.

### Machine-wide installation (Run As Admin - required)

```
LuaInstaller.Console.exe install "dest-dir=C:\Program Files (x86)\Lua" version=5.1.5 env-var=machine
```

> [!NOTE]
> 
> Installs Lua ```5.1.5``` in the ```C:\Program Files (x86)\Lua``` folder and also sets environment variables
machine-wide.