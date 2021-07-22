using System;
using System.IO;
using System.Text;

namespace iff_reader
{
    class Chunk
    {
        readonly BinaryReader _br;
        internal int? bytesLeft;

        internal Chunk(BinaryReader br)
        {
            _br = br;
        }

        static int SwapEndianness(int value)
        {
            var b1 = (value >> 0) & 0xff;
            var b2 = (value >> 8) & 0xff;
            var b3 = (value >> 16) & 0xff;
            var b4 = (value >> 24) & 0xff;

            return b1 << 24 | b2 << 16 | b3 << 8 | b4 << 0;
        }

        internal string GetForm()
        {
            if (bytesLeft is not null) bytesLeft -= 4;
            var value = _br.ReadBytes(4);
            return Encoding.ASCII.GetString(value);
        }

        internal int GetFileSize()
        {
            var value = SwapEndianness(_br.ReadInt32());
            if (bytesLeft is null) bytesLeft = value;
            return value;
        }

        internal string GetRoot()
        {
            return GetForm();
        }

        internal int GetFormSize()
        {
            return SwapEndianness(_br.ReadInt32());
        }
    }
}
