using System;
using System.IO;

namespace iff_reader
{
    class Reader
    {
        readonly string _filePath;

        public Reader(string filePath)
        {
            _filePath = filePath;
        }

        internal void Read()
        {
            using FileStream fs = File.OpenRead(_filePath);
            using BinaryReader br = new(fs);
            Chunk chunk = new(br);
            bool rootRead = false;

            chunk.GetNextChunk();
            chunk.GetFileSize();

            while (chunk.bytesUntilEndOfFile > 0)
            {
                //Console.WriteLine($"Bytes left: {chunk.bytesUntilEndOfFile}");
                if (!rootRead)
                {
                    chunk.GetNextChunk();
                    rootRead = true;
                }

                if (chunk.GetNextChunk() != "FORM")
                {
                    chunk.GetChunkData(chunk.GetChunkSize());
                }
                else
                {
                    chunk.GetChunkSize();
                    chunk.GetNextChunk();
                }
            }
        }
    }
}
