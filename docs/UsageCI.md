# Usage on CI servers

## Overview

By the use of ```LuaInstaller.Console```, you can setup continuous integration / continuous delivery (CI, CD) to buid and test your Lua-related project on Windows against the MSVC toolchain.

## Table of Contents

* [Setup Lua (x86, x64, arm64)](#setup-lua-x86-x64-arm64)
* [Setup Lua and LuaRocks (x86, x64, arm64)](#setup-lua-and-luarocks-x86-x64-arm64)

## Setup Lua (x86, x64, arm64)

Below, in a GitHub workflow, we setup different versions of Lua, building for x86, x64 and arm64 architectures.

> [!TIP]
> 
> After testing the built interpreter, as a bonus, we also make Lua discoverable by ```CMake``` and ```pkg-config```.

```yaml
name: Setup Lua

on: [push, pull_request]

env:
  LUAINSTALLER_VERSION: 0.6.1.0

jobs:

  build:
    runs-on: ${{ matrix.os }}
    name: Build

    strategy:
      matrix:
        include:
          # x86
          - { lua-version: "5.1.5", arch: "x86", os: "windows-latest" }
          - { lua-version: "5.2.4", arch: "x86", os: "windows-latest" }
          - { lua-version: "5.3.6", arch: "x86", os: "windows-latest" }
          - { lua-version: "5.4.8", arch: "x86", os: "windows-latest" }
          # x64
          - { lua-version: "5.1.5", arch: "x64", os: "windows-latest" }
          - { lua-version: "5.2.4", arch: "x64", os: "windows-latest" }
          - { lua-version: "5.3.6", arch: "x64", os: "windows-latest" }
          - { lua-version: "5.4.8", arch: "x64", os: "windows-latest" }
          # arm64
          - { lua-version: "5.1.5", arch: "arm64", os: "windows-11-arm" }
          - { lua-version: "5.2.4", arch: "arm64", os: "windows-11-arm" }
          - { lua-version: "5.3.6", arch: "arm64", os: "windows-11-arm" }
          - { lua-version: "5.4.8", arch: "arm64", os: "windows-11-arm" }

    steps:

      - name: Sanity check the Lua version
        shell: pwsh
        run: |
          if (-not ("${{ matrix.lua-version }}" -match "^[0-9]+\.[0-9]+\.[0-9]+$"))
          {
            Write-Host "Invalid Lua version. It must be on format X.Y.Z";
            exit 1;
          }

      - name: Set environment variable to hold Lua's installation directory
        shell: pwsh
        run: |
          $guid_string = [System.Guid]::NewGuid() | Select-Object -ExpandProperty Guid;
          $lua_dir = Join-Path -Path "${{ runner.temp }}" -ChildPath $guid_string;
          Add-Content "${{ github.env }}" "LUA_DIR=${lua_dir}";

      - name: Download and extract LuaInstaller.Console, and place it on PATH environment variable
        shell: pwsh
        env:
          LUAINSTALLER_URL: https://github.com/luau-project/LuaInstaller/releases/download/v${{ env.LUAINSTALLER_VERSION }}/LuaInstaller.Console-v${{ env.LUAINSTALLER_VERSION }}-${{ matrix.arch }}.zip
        run: |
          $guid_string = [System.Guid]::NewGuid() | Select-Object -ExpandProperty Guid;
          $luainstaller_zip_file = Join-Path -Path "${{ runner.temp }}" -ChildPath "${guid_string}.zip";

          # Download
          Invoke-WebRequest -Uri "${{ env.LUAINSTALLER_URL }}" -OutFile $luainstaller_zip_file;

          # Unzip
          Expand-Archive -Path $luainstaller_zip_file -DestinationPath "${{ runner.temp }}";

          $luainstaller_path = Join-Path -Path "${{ runner.temp }}" -ChildPath "LuaInstaller.Console";

          # Add LuaInstaller.Console to the PATH environment variable
          Add-Content "${{ github.path }}" "${luainstaller_path}";

      - name: Install Lua ${{ matrix.lua-version }}
        run: |
          LuaInstaller.Console install "dest-dir=${{ env.LUA_DIR }}" "version=${{ matrix.lua-version }}" "arch=${{ matrix.arch }}"

      - name: Add Lua interpreter and DLL to PATH environment variable
        shell: pwsh
        run: |
          $lua_bin = Join-Path -Path "${{ env.LUA_DIR }}" -ChildPath "bin";
          Add-Content "${{ github.path }}" "${lua_bin}";

      - name: Test Lua ${{ matrix.lua-version }}
        run: lua -v

      - name: Setup Lua ${{ matrix.lua-version }} for CMake
        shell: pwsh
        run: |
          $cmake_module_path = "$env:CMAKE_PREFIX_PATH";
          Add-Content "${{ github.env }}" "CMAKE_PREFIX_PATH=${{ env.LUA_DIR }};${cmake_module_path}";

      - name: Setup Lua ${{ matrix.lua-version }} for pkg-config
        shell: pwsh
        run: |
          $lua_pc_dir = Get-ChildItem "${{ env.LUA_DIR }}" -File -Recurse |
            Where-Object Name -Like "lua*.pc" |
            Select-Object -ExpandProperty FullName -First 1 |
            Split-Path;

          $pkg_config_path = "$env:PKG_CONFIG_PATH";
          Add-Content "${{ github.env }}" "PKG_CONFIG_PATH=${lua_pc_dir};${pkg_config_path}";
```

## Setup Lua and LuaRocks (x86, x64, arm64)

This time, in another GitHub workflow, we setup Lua and LuaRocks. At the end of the workflow, we install a few libraries using LuaRocks.

```yaml
name: Setup Lua and LuaRocks

on: [push, pull_request]

env:
  LUAINSTALLER_VERSION: 0.7.0.0
  LUAROCKS_VERSION: 3.12.2

jobs:

  build:
    runs-on: ${{ matrix.os }}
    name: Build

    strategy:
      matrix:
        include:
          # x86
          - { lua-version: "5.1.5", arch: "x86", os: "windows-latest" }
          - { lua-version: "5.2.4", arch: "x86", os: "windows-latest" }
          - { lua-version: "5.3.6", arch: "x86", os: "windows-latest" }
          - { lua-version: "5.4.8", arch: "x86", os: "windows-latest" }
          # x64
          - { lua-version: "5.1.5", arch: "x64", os: "windows-latest" }
          - { lua-version: "5.2.4", arch: "x64", os: "windows-latest" }
          - { lua-version: "5.3.6", arch: "x64", os: "windows-latest" }
          - { lua-version: "5.4.8", arch: "x64", os: "windows-latest" }
          # arm64
          - { lua-version: "5.1.5", arch: "arm64", os: "windows-11-arm" }
          - { lua-version: "5.2.4", arch: "arm64", os: "windows-11-arm" }
          - { lua-version: "5.3.6", arch: "arm64", os: "windows-11-arm" }
          - { lua-version: "5.4.8", arch: "arm64", os: "windows-11-arm" }

    steps:

      - name: Sanity check the Lua version
        shell: pwsh
        run: |
          if (-not ("${{ matrix.lua-version }}" -match "^[0-9]+\.[0-9]+\.[0-9]+$"))
          {
            Write-Host "Invalid Lua version. It must be on format X.Y.Z";
            exit 1;
          }

      - name: Set environment variable to hold Lua's installation directory
        shell: pwsh
        run: |
          $guid_string = [System.Guid]::NewGuid() | Select-Object -ExpandProperty Guid;
          $lua_dir = Join-Path -Path "${{ runner.temp }}" -ChildPath $guid_string;
          Add-Content "${{ github.env }}" "LUA_DIR=${lua_dir}";

      - name: Download and extract LuaInstaller.Console, and place it on PATH environment variable
        shell: pwsh
        env:
          LUAINSTALLER_URL: https://github.com/luau-project/LuaInstaller/releases/download/v${{ env.LUAINSTALLER_VERSION }}/LuaInstaller.Console-v${{ env.LUAINSTALLER_VERSION }}-${{ matrix.arch }}.zip
        run: |
          $guid_string = [System.Guid]::NewGuid() | Select-Object -ExpandProperty Guid;
          $luainstaller_zip_file = Join-Path -Path "${{ runner.temp }}" -ChildPath "${guid_string}.zip";

          # Download
          Invoke-WebRequest -Uri "${{ env.LUAINSTALLER_URL }}" -OutFile $luainstaller_zip_file;

          # Unzip
          Expand-Archive -Path $luainstaller_zip_file -DestinationPath "${{ runner.temp }}";

          $luainstaller_path = Join-Path -Path "${{ runner.temp }}" -ChildPath "LuaInstaller.Console";

          # Add LuaInstaller.Console to the PATH environment variable
          Add-Content "${{ github.path }}" "${luainstaller_path}";

      - name: Install Lua ${{ matrix.lua-version }}
        run: |
          LuaInstaller.Console install "dest-dir=${{ env.LUA_DIR }}" "version=${{ matrix.lua-version }}" "arch=${{ matrix.arch }}"

      - name: Add Lua interpreter and DLL to PATH environment variable
        shell: pwsh
        run: |
          $lua_bin = Join-Path -Path "${{ env.LUA_DIR }}" -ChildPath "bin";
          Add-Content "${{ github.path }}" "${lua_bin}";

      - name: Test Lua ${{ matrix.lua-version }}
        run: lua -v

      - name: Setup Lua ${{ matrix.lua-version }} for CMake
        shell: pwsh
        run: |
          $cmake_module_path = "$env:CMAKE_PREFIX_PATH";
          Add-Content "${{ github.env }}" "CMAKE_PREFIX_PATH=${{ env.LUA_DIR }};${cmake_module_path}";

      - name: Setup Lua ${{ matrix.lua-version }} for pkg-config
        shell: pwsh
        run: |
          $lua_pc_dir = Get-ChildItem "${{ env.LUA_DIR }}" -File -Recurse |
            Where-Object Name -Like "lua*.pc" |
            Select-Object -ExpandProperty FullName -First 1 |
            Split-Path;

          $pkg_config_path = "$env:PKG_CONFIG_PATH";
          Add-Content "${{ github.env }}" "PKG_CONFIG_PATH=${lua_pc_dir};${pkg_config_path}";

      # Setting up LuaRocks
      - name: Set environment variable to LuaRocks download URL depending on arch
        shell: pwsh
        run: |
          $allowed_archs = @{ x86 = "32"; x64 = "64"; arm64 = "64" };
          $arch_lower = "${{ matrix.arch }}".ToLower();

          if ($allowed_archs.Keys -contains $arch_lower)
          {
            $v = $allowed_archs[$arch_lower];

            Add-Content "${{ github.env }}" "LUAROCKS_URL=https://luarocks.github.io/luarocks/releases/luarocks-${{ env.LUAROCKS_VERSION }}-windows-${v}.zip";
          }
          else
          {
            Write-Host "Invalid arch: x64, x86 or arm64 expected, but got ${{ matrix.arch }}";
            exit 1;
          }

      - name: Download and extract LuaRocks, and place it on PATH environment variable
        shell: pwsh
        run: |
          $uri = [System.Uri]::new("${{ env.LUAROCKS_URL }}");
          $zipname = $uri | Select-Object -ExpandProperty Segments | Select-Object -Last 1;

          $luarocks_zip = Join-Path -Path "${{ runner.temp }}" -ChildPath "${zipname}";

          # Download
          Invoke-WebRequest -Uri "${{ env.LUAROCKS_URL }}" -OutFile "${luarocks_zip}";

          # Extract
          Expand-Archive -Path "${luarocks_zip}" -DestinationPath "${{ runner.temp }}";

          $luarocks_dir = Get-ChildItem -Path "${{ runner.temp }}" -Recurse -File |
            Where-Object Name -EQ "luarocks.exe" |
            Select-Object -ExpandProperty FullName -First 1 |
            Split-Path;

          # Add LuaRocks to PATH
          Add-Content "${{ github.path }}" "${luarocks_dir}";

      - name: Setup MSVC dev-prompt for LuaRocks configuration
        uses: ilammy/msvc-dev-cmd@v1
        with:
          arch: ${{ matrix.arch }}

      - name: Configure LuaRocks for Lua ${{ matrix.lua-version }}
        shell: pwsh
        run: |
          luarocks config lua_dir "${{ env.LUA_DIR }}";

          $lua_short_version = "${{ matrix.lua-version }}" -split "\." |
            Select-Object -First 2 |
            Join-String -Separator ".";

          luarocks config lua_version $lua_short_version;

          # Update environment variables with variables from LuaRocks
          $luarocks_path = luarocks path;
          Add-Content "${{ github.env }}" $luarocks_path.Replace("""", "").Replace("'", "").Replace("SET ", "");

      - name: Install a few libraries through LuaRocks
        run: |
          luarocks install luafilesystem
          luarocks install luasocket
          luarocks install lua-cjson
```

[Back to the Docs](../docs/README.md)