using System;
using System.IO;

namespace iff_reader
{
    class Reader
    {
        readonly string _filePath;
        readonly IFFFile _iffFile;

        public Reader(string filePath, IFFFile iffFile)
        {
            _filePath = filePath;
            _iffFile = iffFile;
        }

        internal void Read()
        {
            using FileStream fs = File.OpenRead(_filePath);
            using BinaryReader br = new(fs);
            Chunk chunk = new(br, _iffFile);
            bool rootRead = false;

            chunk.GetNextChunk();
            chunk.GetFileSize();

            while (chunk.bytesUntilEndOfFile > 0)
            {
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

            chunk.FinishProcessing();
        }
    }
}
