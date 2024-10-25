using System;
using System.Runtime.InteropServices;

namespace MiniMp3Managed
{
    public static class Mp3Reader
    {


        private static class Bindings
        {
#if PLATFORM_WIN64
            private const string dllName = "minimp3_64";
#elif PLATFORM_WIN32
            private const string dllName = "minimp3";
#elif PLATFORM_POSIX
            private const string dllName = "libminimp3";
#endif
            private const CallingConvention cc = CallingConvention.Cdecl;

            [DllImport(dllName, CallingConvention = cc)]
            public static extern IntPtr mp3i_open_decoder(byte[] buffer, ulong bufferLength);

            [DllImport(dllName, CallingConvention = cc)]
            public static extern void mp3i_delete_decoder(IntPtr decoder);

            [DllImport(dllName, CallingConvention = cc)]
            public static extern ulong mp3i_decode(IntPtr decoder, short[] buffer, ulong bufferLength, ref int error);

            [DllImport(dllName, CallingConvention = cc)]
            public static extern int mp3i_hz(IntPtr decoder);
        }
    }
}
