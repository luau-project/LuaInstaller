﻿using LuaInstaller.Core;
using System;

namespace LuaInstaller.Console
{
    class Program
    {
        private static void Write(string msg)
        {
            System.Console.WriteLine(msg);
        }

        private static void Help()
        {
            Write(@"
------------------------------------------------------
Information
------------------------------------------------------

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

------------------------------------------------------
Installation
------------------------------------------------------

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

------------------------------------------------------
Examples
------------------------------------------------------

1) Installs the latest Lua available on Lua's website
in the current directory, using the latest versions
of Visual Studio and Windows SDK, building the source
code for x64 on 64 Bit Operating Systems or x86 otherwise.

    LuaInstaller.Console.exe install

2) Installs Lua 5.3.6 in the current directory,
using the latest versions of Visual Studio and
Windows SDK, building the source code for x64
platforms

    LuaInstaller.Console.exe install version=5.3.6 arch=x64

3) Installs Lua 5.1.5 in the folder
'C:\Program Files (x86)\Lua',
using the latest versions of Visual Studio and
Windows SDK, building the source code for x64
on 64 Bit Operating Systems or x86 otherwise.
Also sets environment variables machine-wide

------------------------------------------------------
Remark: This kind of machine-wide installation usually
requires 'Administrator' privileges, so you must
'Run As Administrator'
------------------------------------------------------

    LuaInstaller.Console.exe install ""dest-dir=C:\Program Files (x86)\Lua"" version=5.1.5 env-var=machine
");
        }
        
        private static int Install(string[] args)
        {
            int result = 0;

            InstallArguments installArgs = new InstallArguments(new InstalledComponents());
            try
            {
                installArgs.Process(args, 1);

                ICompiler compiler = new VisualStudioCompiler(installArgs.Vs.Toolset.Cl);
                ILinker linker = new VisualStudioLinker(installArgs.Vs.Toolset.Link);

                InstallationManager manager = new InstallationManager(compiler, linker);

                manager.Build(installArgs.Version, installArgs.OutDir, installArgs.Vs, installArgs.Winsdk, installArgs.VariableTarget);
            }
            catch (CliArgumentsException ex)
            {
                Write("Argument error: " + ex.Message);
                result = 3;
            }
            catch (Exception ex)
            {
                Write(ex.Message);
                result = 2;
            }

            return result;
        }

        private static int Main(string[] args)
        {
            int result = 0;
            int nargs = args.Length;
            bool installed = false;

            if (nargs == 0)
            {
                Help();
            }
            else if (nargs == 1)
            {
                string action = args[0];

                switch (action)
                {
                    case "/?":
                    case "help":
                        {
                            Help();

                            break;
                        }
                    case "list-lua":
                        {
                            foreach (LuaVersion v in LuaWebsite.QueryVersions())
                            {
                                Write(v.Version);
                            }
                            break;
                        }
                    case "list-vs":
                    case "list-vs-x86":
                        {
                            IInstalledComponents components = new InstalledComponents();

                            foreach (VisualStudio vs in components.AllVisualStudioX86())
                            {
                                Write(vs.Version.ToString());
                            }
                            break;
                        }
                    case "list-vs-x64":
                        {
                            IInstalledComponents components = new InstalledComponents();

                            foreach (VisualStudio vs in components.AllVisualStudioX64())
                            {
                                Write(vs.Version.ToString());
                            }
                            break;
                        }
                    case "list-win-sdk":
                        {
                            IInstalledComponents components = new InstalledComponents();

                            foreach (WindowsSdk vs in components.AllWindowsSdkX86())
                            {
                                Write(vs.Version.ToString());
                            }
                            break;
                        }
                    case "install":
                        {
                            result = Install(args);
                            installed = result == 0;
                            break;
                        }
                    default:
                        {
                            Write("Unknown option");
                            result = 1;
                            break;
                        }
                }
            }
            else
            {
                string action = args[0];

                switch (action)
                {
                    case "install":
                        {
                            result = Install(args);
                            installed = result == 0;
                            break;
                        }
                    default:
                        {
                            Write("Unknown option");
                            result = 1;
                        }
                        break;
                }
            }
            
            if (installed)
            {
                Write("Lua was installed successfully.");
            }

            return result;
        }
    }
}
