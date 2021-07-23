using System.Collections.Generic;

namespace iff_reader
{
    public class IFFFile
    {
        public string FileName { get; set; }
        public int FileSize { get; set; }
        // public List<string> NextChunk {  get; set; }
        public int Slots { get; set; }
        public int Attributes { get; set; }
        public List<string> IngredientTemplateName { get; set; }
        public List<string> IngredientTitleName { get; set; }
        public List<string> ExperimentalSubGroupTitle { get; set; }
        public List<string> ExperimentalGroupTitle { get; set; }
        public List<int> MinValue { get; set; }
        public List<int> MaxValue { get; set; }
        public string CraftedSharedTemplate { get; set; }
    }
}
