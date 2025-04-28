# LuaInstaller

[![CI](https://github.com/luau-project/LuaInstaller/actions/workflows/CI.yaml/badge.svg)](./.github/workflows/CI.yaml)
[![github release](https://img.shields.io/github/release/luau-project/LuaInstaller.svg?logo=github)](https://github.com/luau-project/LuaInstaller/releases/latest)

[![icon](./LuaInstaller/Assets/LuaInstaller-256x256.ico)](#overview)

## Overview

The toolset available here aids in the process to download, build and install Lua versions on Windows, downloading Lua source code directly from [https://lua.org](https://lua.org).

> [!NOTE]
> 
> To build Lua for x86 or x64, you must be running a standard 64-bit Windows (no ARM64). Moreover, you are required to be running 64-bit Windows on ARM to build Lua targeting the ARM64 architecture.

## Table of Contents
* [For End-Users](#for-end-users)
    * [Setup](#setup)
    * [Running](#running)
* [For Developers](#for-developers)
    * [Running from the command line](#running-from-the-command-line)
* [Contributors](#contributors)

## For End-Users

### Setup

> [!TIP]
> 
> Among all the possible versions of Visual Studio, **Visual Studio Community** edition or **Visual Studio Build Tools** are the preferred choices for individuals.

1. Visit [https://visualstudio.microsoft.com](https://visualstudio.microsoft.com), download and install the most recent Microsoft Visual Studio C/C++ Build Tools for native desktop development (MSVC), together with the most recent Windows SDK for your operating system:
    * **Step 1**: In the startup screen of the latest ```Visual Studio Installer```, switch to ```Individual components``` tab in the top;
    
    ![Step 1: Switch to Individual components tab](https://github.com/user-attachments/assets/f238d870-3ce9-4f69-8539-3c4484e08ec2)
    
    * **Step 2**: Search for ```Windows SDK```;
    * **Step 3**: Select the most recent;
    
    ![Step 2: Search for "Windows SDK"; Step 3: Select the most recent;](https://github.com/user-attachments/assets/5e45e783-129c-484e-a0e4-f557da6e8d5f)
    
    * **Step 4**: Search for ```latest MSVC Build Tools```;
    * **Step 5**: Select the ```latest``` MSVC build tools available;

    > <br>
    > In the image below, the latest x86/x64 build tools was chosen because I am running a vanilla x64 Windows. If you are running 64-bit Windows on ARM, then tick the ARM64/ARM64EC build tools option.

    * **Step 6**: In the right panel, verify that the ```Individual components``` selection shows the latest Build Tools and Windows SDK;
    * **Step 7**: Hit ```Install``` button and await the installation.
    
    ![Step 4: Search for latest MSVC Build Tools; Step 5: Select the latest MSVC x86/x64 build tools available; Step 6: In the right panel, verify that the Individual components selection shows the latest Build Tools and Windows SDK; Step 7: Hit Install button and await the installation.](https://github.com/user-attachments/assets/b84e383e-fb2e-48cb-86cd-132b33b0a1fc)

2. Download and extract the latest ```LuaInstaller.EndUsers-*.zip``` from the [Releases](https://github.com/luau-project/LuaInstaller/releases/latest) page anywhere on your computer;

### Running

1. Run the app ```LuaInstaller.exe```

> [!IMPORTANT]
> 
> * If you want to install Lua on system-wide directories like ```C:\Program Files\Lua``` or ```C:\Program Files(x86)\Lua```, close the program and open ```LuaInstaller.exe``` again as administrator (Run as Administrator);
> * To set environment variables, you are also required to have admin privileges (Run as Administrator).

2. If everything was configured correctly, you should see each dropdown with at least one choice available:

![running](https://github.com/user-attachments/assets/47d03000-3036-403e-8fcc-3f9922ebba86)

3. Make your choices and hit ```Install```;
4. Verify that you can find ```lua.exe``` at ```FOLDER > bin```, where FOLDER means the destination directory for Lua installation.

## For Developers

* Are you looking for CI / CD content? [check our CI examples here](./docs/UsageCI.md).

* Do you want to integrate it on your project? Read the [docs](./docs/README.md).

### Running from the command line

1. Download and extract the latest ```LuaInstaller.Console-*.zip``` that suits better your operating system from the [Releases](https://github.com/luau-project/LuaInstaller/releases/latest) page anywhere on your computer;
2. Open a command prompt (`cmd`) and change directory to the folder of `LuaInstaller.Console`;
3. Install a required Lua version

   ```batch
   LuaInstaller.Console.exe install "version=5.4.7" "dest-dir=C:\Lua-5.4.7"
   ```

   ![running-from-cmd](https://github.com/user-attachments/assets/eaf0f4cf-7dd8-4e23-933a-b2290dea93ba)

> [!IMPORTANT]
> 
> * If you want to install Lua on system-wide directories like ```C:\Program Files\Lua``` or ```C:\Program Files(x86)\Lua```, close the command prompt and open ```cmd``` again as administrator (Run as Administrator);
> * To set environment variables, you are also required to have admin privileges on `cmd` (Run as Administrator).

4. Enjoy!

## Contributors

Special thanks goes to [warlockx](https://github.com/Warlockx) for valuable suggestions and bug hunting in the beginning of this project.
