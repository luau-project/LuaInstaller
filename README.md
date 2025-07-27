# LuaInstaller

[![CI](https://github.com/luau-project/LuaInstaller/actions/workflows/CI.yaml/badge.svg)](./.github/workflows/CI.yaml)
[![github release](https://img.shields.io/github/release/luau-project/LuaInstaller.svg?logo=github)](https://github.com/luau-project/LuaInstaller/releases/latest)

[![icon](./LuaInstaller/Assets/LuaInstaller-256x256.ico)](#overview)

## Overview

The toolset available here aids in the process to download, build and install Lua versions on Windows, downloading Lua source code directly from [https://lua.org](https://lua.org).

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

<img width="821" height="684" alt="running" src="https://github.com/user-attachments/assets/f23284f4-0781-422c-9ee2-6cc133d6046d" />

> [!NOTE]
> 
> Only native builds of Lua are possible (no cross-compilation). This means that you are required to be running a 64-bit Windows on ARM to build Lua targeting the ARM64 architecture.

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
   LuaInstaller.Console.exe install "version=5.4.8" "dest-dir=C:\Lua-5.4.8"
   ```

   <img width="1430" height="505" alt="running-from-cmd" src="https://github.com/user-attachments/assets/a7927717-ff87-4719-98f2-2e28e7f36031" />

> [!IMPORTANT]
> 
> * If you want to install Lua on system-wide directories like ```C:\Program Files\Lua``` or ```C:\Program Files(x86)\Lua```, close the command prompt and open ```cmd``` again as administrator (Run as Administrator);
> * To set environment variables, you are also required to have admin privileges on `cmd` (Run as Administrator).

4. Enjoy!

## Contributors

Special thanks goes to [warlockx](https://github.com/Warlockx) for valuable suggestions and bug hunting in the beginning of this project.
