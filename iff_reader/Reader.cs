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
                if (!rootRead)
                {
                    chunk.GetRoot();                                         
                    rootRead = true;
                }
                
                string data = chunk.GetNextChunk();

                if (data != "FORM")
                {
                    int chunkSize = chunk.GetChunkSize();
                    chunk.GetChunkData(chunkSize);
                }
                else
                {
                    chunk.GetChunkSize();
                    chunk.GetChunkName();
                }
            }
        }
    }
}
