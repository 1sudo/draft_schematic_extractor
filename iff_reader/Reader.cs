using System;
using System.IO;

namespace iff_reader
{
    class Reader
    {
        readonly string _filePath;
        Chunk _chunk;

        public Reader(string filePath)
        {
            _filePath = filePath;
        }

        internal void Read()
        {
            using FileStream fs = File.OpenRead(_filePath);
            using BinaryReader br = new(fs);

            _chunk = new(br);

            _chunk.GetForm();
            _chunk.GetFileSize();
            _chunk.GetRoot();
            _chunk.GetForm();
            _chunk.GetFormSize();
            _chunk.GetForm();
            
            int formSize = _chunk.GetFormSize();




            // Need check to ensure not end of stream
            // While bytesLeft > 1



            Console.WriteLine();
        }
    }
}
