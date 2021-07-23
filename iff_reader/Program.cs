using Newtonsoft.Json;
using System.Collections.Generic;
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
        }

        void Run()
        {
            iffFile = new();

            string fileName = "shared_sword_ryyk_blade.iff";
            iffFile.FileName = fileName;

            Reader reader = new(fileName, iffFile);
            reader.Read();

            File.WriteAllText("test.json", JsonConvert.SerializeObject(iffFile, Formatting.Indented));
        }

        internal void OnFileSize(int fileSize, IFFFile iffFile)
        {
            iffFile.FileSize = fileSize;
        }

        internal void OnNextChunk(List<string> chunkNames, IFFFile iffFile)
        {
            // iffFile.NextChunk = chunkNames;
        }

        internal void OnSlots(int slots, IFFFile iffFile)
        {
            iffFile.Slots = slots;
        }

        internal void OnAttributes(int attributes, IFFFile iffFile)
        {
            iffFile.Attributes = attributes;
        }

        internal void OnIngredientTemplateName(List<string> templateNames, IFFFile iffFile)
        {
            iffFile.IngredientTemplateName = templateNames;
        }

        internal void OnIngredientTitleName(List<string> titleNames, IFFFile iffFile)
        {
            iffFile.IngredientTitleName = titleNames;
        }

        internal void OnExperimentalSubGroupTitle(List<string> subGroupTitles, IFFFile iffFile)
        {
            iffFile.ExperimentalSubGroupTitle = subGroupTitles;
        }

        internal void OnExperimentalGroupTitle(List<string> groupTitles, IFFFile iffFile)
        {
            iffFile.ExperimentalGroupTitle = groupTitles;
        }

        internal void OnMinValue(List<int> minValues, IFFFile iffFile)
        {
            iffFile.MinValue = minValues;
        }

        internal void OnMaxValue(List<int> maxValues, IFFFile iffFile)
        {
            iffFile.MaxValue = maxValues;
        }

        internal void OnCraftedSharedTemplate(string template, IFFFile iffFile)
        {
            iffFile.CraftedSharedTemplate = template;
        }
    }
}
