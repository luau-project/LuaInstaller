$dotnet_dir = Join-Path -Path (Get-Location) -ChildPath "dotnet_dir";
$dotnet_install_file = Join-Path -Path $dotnet_dir -ChildPath "dotnet-install.ps1";
$dotnet_runner = Join-Path -Path $dotnet_dir -ChildPath "dotnet";

Invoke-WebRequest -Uri 'https://dot.net/v1/dotnet-install.ps1' -UseBasicParsing -OutFile $dotnet_install_file;
& $dotnet_install_file -Architecture x64 -Version "6.0.301" -InstallDir $dotnet_dir;

$archs = 'x64', 'x86';

foreach ($arch in $archs) {
    Remove-Item .\LuaInstaller.Console\bin, .\LuaInstaller.Console\obj, .\LuaInstaller\bin, .\LuaInstaller\obj -Recurse;
    
    & $dotnet_runner restore LuaInstaller.sln;
    & $dotnet_runner build .\LuaInstaller.Console\LuaInstaller.Console.csproj -c Release -r win-$arch --self-contained true;
    & $dotnet_runner build .\LuaInstaller\LuaInstaller.csproj -c Release -r win-$arch --self-contained true;

    $console = ".\LuaInstaller.Console\bin\Release\netcoreapp3.0\win-$arch\LuaInstaller.Console.exe";

    $commands = "/?", "list-win-sdk", "list-vs-$arch", "list-lua";

    foreach ($c in $commands) {
        Write-Host;
        Write-Host command: $c -ForegroundColor Cyan;
        & $console $c;
        Write-Host;
    }

    $lua_versions = "5.1.5", "5.2.4", "5.3.6", "5.4.4";

    $current_dir = Get-Location;

    foreach ($lua_ver in $lua_versions) {
        Write-Host;
        Write-Host Installing Lua $lua_ver;
        $lua_ver_dir = Join-Path -Path $current_dir -ChildPath $lua_ver | Join-Path -ChildPath $arch;

        & $console install "dest-dir=$lua_ver_dir" version=$lua_ver arch=$arch;
        
        $lua_exe_dir = Join-Path -Path $lua_ver_dir -ChildPath 'bin';
        $lua_exe = Join-Path -Path $lua_exe_dir -ChildPath 'lua.exe';
        Write-Host Executable file: $lua_exe;

        Write-Host;
        Write-Host Testing Lua:
        Write-Host;
        & $lua_exe -v;

        Write-Host;
    }
}
