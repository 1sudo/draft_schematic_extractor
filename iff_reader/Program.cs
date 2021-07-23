using Newtonsoft.Json;
using System;
using System.IO;

namespace iff_reader
{
    class Program
    {
        IFFFile iffFile;

        static void Main(string[] args)
        {
            Program program = new();

            program.Run();

            Chunk.OnFileSize += program.OnFileSize;
            Chunk.OnNextChunk += program.OnNextChunk;
            Chunk.OnSlots += program.OnSlots;
            Chunk.OnAttributes += program.OnAttributes;
            Chunk.OnIngredientTemplateName += program.OnIngredientTemplateName;
            Chunk.OnIngredientTitleName += program.OnIngredientTitleName;
            Chunk.OnExperimentalSubGroupTitle += program.OnExperimentalSubGroupTitle;
            Chunk.OnExperimentalGroupTitle += program.OnExperimentalGroupTitle;
            Chunk.OnMinValue += program.OnMinValue;
            Chunk.OnMaxValue += program.OnMaxValue;
        }

        void Run()
        {
            iffFile = new();
            Reader reader = new("shared_sword_ryyk_blade.iff");
            reader.Read();
            File.WriteAllText("test.json", JsonConvert.SerializeObject(iffFile));
        }

        void OnFileSize(int fileSize)
        {
            Console.WriteLine("test");
            iffFile.FileSize = fileSize;
        }

        void OnNextChunk(string chunkName)
        {
            iffFile.NextChunk = chunkName;
        }

        void OnSlots(int slots)
        {
            iffFile.Slots = slots;
        }

        void OnAttributes(int attributes)
        {
            iffFile.Attributes = attributes;
        }

        void OnIngredientTemplateName(string templateName)
        {
            iffFile.IngredientTemplateName = templateName;  
        }
        
        void OnIngredientTitleName(string titleName)
        {
            iffFile.IngredientTitleName = titleName;
        }

        void OnExperimentalSubGroupTitle(string subGroupTitle)
        {
            iffFile.ExperimentalSubGroupTitle = subGroupTitle;
        }

        void OnExperimentalGroupTitle(string groupTitle)
        {
            iffFile.ExperimentalGroupTitle = groupTitle;
        }

        void OnMinValue(int minValue)
        {
            iffFile.MinValue = minValue;
        }

        void OnMaxValue(int maxValue)
        {
            iffFile.MaxValue = maxValue;
        }
    }
}
