![CI](https://github.com/luau-project/LuaInstaller/actions/workflows/CI.yaml/badge.svg)
[![github release](https://img.shields.io/github/release/luau-project/LuaInstaller.svg?logo=github)](https://github.com/luau-project/LuaInstaller/releases/latest)

## Overview

The toolset available here aids the process to download, build and install Lua versions on Windows, downloading Lua source code directly from [https://www.lua.org](https://www.lua.org). 

## Table of Contents
* [For End-Users](#for-end-users)
    * [Setup](#setup)
    * [Running](#running)
* [For Developers](#for-developers)
* [Future Work](#future-work)
* [Contributors](#contributors)

## For End-Users

### Setup

> [!TIP]
> 
> Among all the possible versions of Visual Studio, **Visual Studio Community** edition or **Visual Studio Build Tools** are the preferred choices for individuals.

1. Visit [https://visualstudio.microsoft.com](https://visualstudio.microsoft.com), download and install the most recent MSVC (Microsoft Visual Studio for C/C++) Build Tools for x86/x64 native desktop development together with the most recent Windows SDK for your operating system:
    * Step 1: On the latest Visual Studio installer, switch to individual components tab;
    
    ![Step 1: Switch to Individual Components tab](https://github.com/user-attachments/assets/8f848c4e-d61d-4fc7-a869-a0b63fb4aecb)
    
    * Step 2: Search for "Windows SDK"; Step 3: Select the most recent;
    
    ![Step 2: Search for "Windows SDK"; Step 3: Select the most recent;](https://github.com/user-attachments/assets/04a3a6a6-297c-4935-a5b7-0baa41cddc12)
    
    * Step 4: Search for "MSVC Build Tools"; Step 5: Select the latest x86/x64 build tools; Step 6: Verify the "Individual Components" selection shows the latest Build Tools and Windows SDK; Step 7: Hit "Install" button and await the installation.
    
    ![Step 4: Search for MSVC Build Tools; Step 5: Select the latest x86/x64 build tools; Step 6: Verify that the Individual Components selection shows the latest Build Tools and Windows SDK; Step 7: Hit Install button and await the installation.](https://github.com/user-attachments/assets/7c3cec11-31d5-4bc7-85bb-08ba53b655c1)

2. Download and extract the latest ```LuaInstaller.EndUsers-*.zip``` from the [Releases](https://github.com/luau-project/LuaInstaller/releases) page anywhere on your computer;

### Running

1. Run the app ```LuaInstaller.exe```

> [!IMPORTANT]
> 
> * If you want to install Lua on system-wide directories like ```C:\Program Files\Lua``` or ```C:\Program Files(x86)\Lua```, close the program and open ```LuaInstaller.exe``` again as administrator (Run as Administrator);
> * To set environment variables, you are also required to have admin privileges (Run as Administrator).

2. If everything was configured correctly, you should see each dropdown with at least one choice available:

![running](https://github.com/user-attachments/assets/20038d77-33e3-47e7-aca4-06e9c09514eb)

3. Make your choices and hit ```Install```;
4. Verify that you can find ```lua.exe``` at ```FOLDER > bin```, where FOLDER means the destination directory for Lua installation.

## For Developers

Do you want to integrate it on your project? Read the [docs](./docs/README.md).

## Future Work

We want to ease the process to have latest versions of Lua and LuaRocks installed in the system, so LuaRocks support is on top of our todo list.

The following features might come in next releases
* Logging
* LuaRocks installation
* Full rewrite of the project

## Contributors

Special thanks goes to [warlockx](https://github.com/Warlockx) for bug hunting and valuable suggestions that are going to be incorporated soon.
