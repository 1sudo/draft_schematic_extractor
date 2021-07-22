using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace iff_reader
{
    class Chunk
    {
        readonly BinaryReader _br;
        internal int? bytesUntilEndOfFile;
        internal int? bytesUntilEndOfForm;

        internal Chunk(BinaryReader br)
        {
            _br = br;
        }

        internal string GetType(string text)
        {
            switch (text)
            {
                case "XXXX": return "CHUNK";
                case "PCNT": return "CHUNK";
                default: return "FORM";
            }
        }

        static int SwapEndianness(int value)
        {
            var b1 = (value >> 0) & 0xff;
            var b2 = (value >> 8) & 0xff;
            var b3 = (value >> 16) & 0xff;
            var b4 = (value >> 24) & 0xff;

            return b1 << 24 | b2 << 16 | b3 << 8 | b4 << 0;
        }

        internal string GetNextChunk()
        {
            if (bytesUntilEndOfFile is not null) bytesUntilEndOfFile -= 4;
            var value = _br.ReadBytes(4);
            Console.WriteLine(Encoding.ASCII.GetString(value));
            return Encoding.ASCII.GetString(value);
        }

        internal string GetChunkName()
        {
            return GetNextChunk();
        }

        internal int GetFileSize()
        {
            var value = SwapEndianness(_br.ReadInt32());
            if (bytesUntilEndOfFile is null) bytesUntilEndOfFile = value;
            Console.WriteLine(value);
            return value;
        }

        internal string GetRoot()
        {
            return GetNextChunk();
        }

        internal int GetChunkSize()
        {
            int value = SwapEndianness(_br.ReadInt32());
            bytesUntilEndOfForm = value;
            bytesUntilEndOfFile -= 4;
            Console.WriteLine(value);
            return value;
        }

        internal string GetChunkData(int chunkSize)
        {
            List<byte> data = new();

            for (int i = 0; i < chunkSize; i++)
            {
                data.Add(_br.ReadByte());
                bytesUntilEndOfForm -= 1;
                bytesUntilEndOfFile -= 1;
            }

            Console.WriteLine(Encoding.UTF8.GetString(data.ToArray()));
            return Encoding.UTF8.GetString(data.ToArray());
        }
    }
}
