using System;
using System.Runtime.InteropServices;

namespace MiniMp3Managed
{
    public class Mp3Reader : IDisposable
    {
        private bool disposed;
        private readonly IntPtr decoderPtr;
        private short[] internalBuffer;

        private readonly ulong samples;
        private readonly int sampleRate;
        private readonly int channels;
        private readonly int bitrate;
        private readonly TimeSpan duration;

        public ulong Samples => samples;
        public int SampleRate => sampleRate;
        public int Channels => channels;
        public int Bitrate => bitrate;
        public TimeSpan Duration => duration;
        public bool Error { get; private set; }

        public Mp3Reader(byte[] data)
        {
            decoderPtr = Bindings.OpenDecoder(data, (ulong)data.Length);
            if(decoderPtr == IntPtr.Zero)
            {
                throw new Exception("Native decoder could not be created!");
            }
            internalBuffer = new short[4096];
            Bindings.GetStreamInfo(decoderPtr, ref samples, ref sampleRate, ref channels, ref bitrate);
            if(sampleRate != 0 && channels != 0)
            {
                duration = TimeSpan.FromSeconds(samples / (double)sampleRate / channels);
            }
        }

        ~Mp3Reader()
        {
            Dispose(false);
        }

        public int ReadSamples(float[] buffer, int count)
        {
            if(internalBuffer.Length < count)
            {
                Array.Resize(ref internalBuffer, count);
            }

            int error = 0;
            int read = (int)Bindings.ReadSamples(decoderPtr, internalBuffer, (ulong)count, ref error);
            Error |= error != 0;

            const float shortMax = -short.MinValue;
            const float mult = 1f / shortMax;
            for(int i = 0;i < read;i++)
            {
                buffer[i] = internalBuffer[i] * mult;
            }
            return read;
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(decoderPtr != IntPtr.Zero)
                {
                    Bindings.CloseDecoder(decoderPtr);
                }
                internalBuffer = null;
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static class Bindings
        {
#if PLATFORM_WIN64
            private const string dllName = "minimp3_64.dll";
#elif PLATFORM_WIN32
            private const string dllName = "minimp3.dll";
#elif PLATFORM_POSIX
            private const string dllName = "libminimp3.so";
#endif
            private const CallingConvention cc = CallingConvention.Cdecl;

            [DllImport(dllName, CallingConvention = cc, EntryPoint = "mp3i_open_decoder")]
            public static extern IntPtr OpenDecoder(byte[] buffer, ulong bufferLength);

            [DllImport(dllName, CallingConvention = cc, EntryPoint = "mp3i_close_decoder")]
            public static extern void CloseDecoder(IntPtr decoder);

            [DllImport(dllName, CallingConvention = cc, EntryPoint = "mp3i_read_samples")]
            public static extern ulong ReadSamples(IntPtr decoder, short[] buffer, ulong bufferLength, ref int error);

            [DllImport(dllName, CallingConvention = cc, EntryPoint = "mp3i_stream_info")]
            public static extern void GetStreamInfo(IntPtr decoder, ref ulong samples, ref int sampleRate, ref int channels, ref int bitrate);
        }
    }
}
