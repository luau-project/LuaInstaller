using System;
#if !NET6_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace LuaInstaller.Core
{
    public class ArchitectureSelector
    {
        private static readonly ArchitectureSelector _instance;
        static ArchitectureSelector()
        {
            _instance = new ArchitectureSelector();
        }

        public static ArchitectureSelector Instance
        {
            get
            {
                return _instance;
            }
        }

        private const int UNINITIALIZED_ARCH = -1;
        private int _arch;
        private ArchitectureSelector()
        {
            _arch = UNINITIALIZED_ARCH;
        }

#if NET6_0_OR_GREATER
        public Architecture Architecture
        {
            get
            {
                if (_arch == UNINITIALIZED_ARCH)
                {
                    Architecture result = Architecture.X86;

                    System.Runtime.InteropServices.Architecture architecture = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture;
                    switch (architecture)
                    {
                        case System.Runtime.InteropServices.Architecture.Arm64:
                            {
                                result = Architecture.ARM64;
                                break;
                            }
                        case System.Runtime.InteropServices.Architecture.X64:
                            {
                                result = Architecture.X64;
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    _arch = (int)result;
                }

                return (Architecture)_arch;
            }
        }
#else

        internal static class NativeMethods
        {
            public const string KERNEL32 = "kernel32.dll";
            public const uint LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800;

            public const ushort IMAGE_FILE_MACHINE_AMD64 = 0x8664;
            public const ushort IMAGE_FILE_MACHINE_ARM64 = 0xAA64;
            public const ushort IMAGE_FILE_MACHINE_I386 = 0x014C;
            public const ushort IMAGE_FILE_MACHINE_ARMNT = 0x01C4;

            public const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;
            public const ushort PROCESSOR_ARCHITECTURE_ARM64 = 12;

            [DllImport(KERNEL32)]
            public static extern IntPtr LoadLibraryExW(
                [In][MarshalAs(UnmanagedType.LPWStr)] string lpLibFileName,
                IntPtr hFile,
                [In] uint dwFlags
            );

            [DllImport(KERNEL32)]
            public static extern IntPtr GetProcAddress(
                [In] IntPtr hModule,
                [In] [MarshalAs(UnmanagedType.LPStr)] string lpProcName
            );

            [DllImport(KERNEL32)]
            public static extern IntPtr GetCurrentProcess();

            public delegate bool IsWow64Process2FunctionPointer([In] IntPtr hProcess, [In, Out] IntPtr pProcessMachine, [In, Out] IntPtr pNativeMachine);

            [StructLayout(LayoutKind.Sequential)]
            public struct DUMMYSTRUCTNAME
            {
                public ushort wProcessorArchitecture;
                public ushort wReserved;
            }

            [StructLayout(LayoutKind.Explicit)]
            public struct DUMMYUNIONNAME
            {
                [FieldOffset(0)]
                public uint dwOemId;

                [FieldOffset(0)]
                public DUMMYSTRUCTNAME DUMMYSTRUCTNAME;
            }

            [StructLayout(LayoutKind.Sequential)]
            public class SYSTEM_INFO
            {
                public DUMMYUNIONNAME DUMMYUNIONNAME;
                public uint dwPageSize;
                public IntPtr lpMinimumApplicationAddress;
                public IntPtr lpMaximumApplicationAddress;
                public IntPtr dwActiveProcessorMask;
                public uint dwNumberOfProcessors;
                public uint dwProcessorType;
                public uint dwAllocationGranularity;
                public ushort wProcessorLevel;
                public ushort wProcessorRevision;
            }

            [DllImport(KERNEL32)]
            public static extern void GetNativeSystemInfo(SYSTEM_INFO lpSystemInfo);
        }

        public LuaInstaller.Core.Architecture Architecture
        {
            get
            {
                if (_arch == UNINITIALIZED_ARCH)
                {
                    LuaInstaller.Core.Architecture result = LuaInstaller.Core.Architecture.X86;

                    IntPtr kernel32HModule = NativeMethods.LoadLibraryExW(NativeMethods.KERNEL32, IntPtr.Zero, NativeMethods.LOAD_LIBRARY_SEARCH_SYSTEM32);
                    if (kernel32HModule != IntPtr.Zero)
                    {
                        IntPtr IsWow64Process2HModule = NativeMethods.GetProcAddress(kernel32HModule, "IsWow64Process2");

                        if (IsWow64Process2HModule == IntPtr.Zero)
                        {
                            NativeMethods.IsWow64Process2FunctionPointer IsWow64Process = Marshal.GetDelegateForFunctionPointer
                                <NativeMethods.IsWow64Process2FunctionPointer>(IsWow64Process2HModule);

                            IntPtr mem = IntPtr.Zero;
                            
                            try
                            {
                                mem = Marshal.AllocHGlobal(2 * sizeof(ushort));
                                
                                if (mem != IntPtr.Zero)
                                {
                                    IntPtr pProcessMachinePtr = mem;
                                    IntPtr pNativeMachinePtr = IntPtr.Add(mem, sizeof(ushort));

                                    if (IsWow64Process(NativeMethods.GetCurrentProcess(), pProcessMachinePtr, pNativeMachinePtr))
                                    {
                                        ushort pProcessMachine = (ushort)Marshal.ReadInt16(pProcessMachinePtr);
                                        ushort pNativeMachine = (ushort)Marshal.ReadInt16(pNativeMachinePtr);

                                        switch (pProcessMachine)
                                        {
                                            case NativeMethods.IMAGE_FILE_MACHINE_AMD64:
                                                {
                                                    result = LuaInstaller.Core.Architecture.X64;
                                                    break;
                                                }
                                            case NativeMethods.IMAGE_FILE_MACHINE_ARM64:
                                                {
                                                    result = LuaInstaller.Core.Architecture.ARM64;
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
#if TARGET_AMD64
                                        result = LuaInstaller.Core.Architecture.ARM64;
#else
                                        result = Environment.Is64BitOperatingSystem ? LuaInstaller.Core.Architecture.X64 : LuaInstaller.Core.Architecture.X86;
#endif
                                    }
                                }
                            }
                            finally
                            {
                                if (mem != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(mem);
                                }
                            }
                        }
                        else // failed to find IsWow64Process2 on kernel32
                        {
                            NativeMethods.SYSTEM_INFO info = new NativeMethods.SYSTEM_INFO();
                            NativeMethods.GetNativeSystemInfo(info);

                            switch (info.DUMMYUNIONNAME.DUMMYSTRUCTNAME.wProcessorArchitecture)
                            {
                                case NativeMethods.PROCESSOR_ARCHITECTURE_AMD64:
                                    {
                                        result = LuaInstaller.Core.Architecture.X64;
                                        break;
                                    }
                                case NativeMethods.PROCESSOR_ARCHITECTURE_ARM64:
                                    {
                                        result = LuaInstaller.Core.Architecture.ARM64;
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                    }

                    _arch = (int)result;
                }

                return (LuaInstaller.Core.Architecture)_arch;
            }
        }
#endif
    }
}
