$console = 'LuaInstaller.Console\bin\Release\LuaInstaller.Console.exe';

$infoCommands = '/?',
			'help',
			'list-vs',
			'list-vs-x86',
			'list-vs-x64',
			'list-win-sdk';

Write-Host 'empty command (no args)' -ForegroundColor Cyan;

& $console;

foreach ($c in $infoCommands) {
    Write-Host;
    Write-Host command: $c -ForegroundColor Cyan;
    Write-Host;
	& $console $c;
    Write-Host;
}

$destDir = 'lua';
$versions = '5.1.5', '5.2.4', '5.3.3';

$luaExe = Join-Path -Path $destDir -ChildPath "bin" | Join-Path -ChildPath 'lua.exe';

foreach ($v in $versions) {
    Write-Host;
    Write-Host command: install dest-dir=$destDir version=$v -ForegroundColor Cyan;
    Write-Host;
	& $console install dest-dir=$destDir version=$v;
    Write-Host;
    Write-Host 'Testing lua interpreter' -ForegroundColor Cyan;
    Write-Host;
    & $luaExe -v;
    Write-Host;
    Write-Host 'Cleaning installation' -ForegroundColor Cyan;
    Write-Host;
    Remove-Item -Path $destDir -Recurse -Force;
    Write-Host;
}