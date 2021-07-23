namespace iff_reader
{
    internal class IFFFile
    {
        internal string FileName { get; set; }
        internal int FileSize { get; set; }
        internal string NextChunk {  get; set; }
        internal int Slots { get; set; }
        internal int Attributes { get; set; }
        internal string IngredientTemplateName { get; set; }
        internal string IngredientTitleName { get; set; }
        internal string ExperimentalSubGroupTitle { get; set; }
        internal string ExperimentalGroupTitle { get; set; }
        internal int MinValue { get; set; }
        internal int MaxValue { get; set; }
    }
}
