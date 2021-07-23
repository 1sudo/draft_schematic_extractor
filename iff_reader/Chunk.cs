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
        Program _program;
        IFFFile _iffFile;
        List<string> chunks;
        List<string> ingredientTemplateNames;
        List<string> ingredientTitleNames;
        List<string> experimentalSubGroupTitles;
        List<string> experimentalGroupTitles;
        List<int> minValues;
        List<int> maxValues;

        internal Chunk(BinaryReader br, IFFFile iffFile)
        {
            _br = br;
            _program = new();
            _iffFile = iffFile;

            chunks = new();
            ingredientTemplateNames = new();
            ingredientTitleNames = new();
            experimentalSubGroupTitles = new();
            experimentalGroupTitles = new();
            minValues = new();
            maxValues = new();
        }

        internal string GetNextChunk()
        {
            if (bytesUntilEndOfFile is not null) bytesUntilEndOfFile -= 4;
            var value = _br.ReadBytes(4);
            // chunks.Add(Encoding.ASCII.GetString(value));
            return Encoding.ASCII.GetString(value);
        }

        internal int GetFileSize()
        {
            var value = Utils.SwapEndianness(_br.ReadInt32());
            if (bytesUntilEndOfFile is null) bytesUntilEndOfFile = value;
            _program.OnFileSize(value, _iffFile);
            return value;
        }

        internal int GetChunkSize()
        {
            int value = Utils.SwapEndianness(_br.ReadInt32());
            bytesUntilEndOfFile -= 4;
            return value;
        }

        internal void CheckChunkFor(string chunkType, string data)
        {
            if (chunkType == "slots" || chunkType == "attributes")
            {
                if (data.Contains(chunkType))
                {
                    StringBuilder sb = new();

                    foreach (char c in data)
                    {
                        if (!char.IsLetter(c))
                        {
                            sb.Append(c);
                        }
                    }

                    string nextChunk = Utils.Reverse(data.Split(sb.ToString())[1]);

                    if (chunkType == "slots")
                    {
                        int slots = Utils.StringToDecimal(sb.ToString().Substring(2, 4));
                        _program.OnSlots(slots, _iffFile);
                    }
                    if (chunkType == "attributes")
                    {
                        int attributes = Utils.StringToDecimal(sb.ToString().Substring(2, 4));
                        _program.OnAttributes(attributes, _iffFile);
                    }
                }
            }

            if (chunkType == "name")
            {
                if (data.Contains(chunkType) && data.Contains("craft_"))
                {
                    bool badChar = true;
                    StringBuilder sb = new();
                    string ingredientTemplateName = "";
                    string ingredientTitleName = "";
                    int varType = 0;

                    foreach (char c in data)
                    {
                        if (char.IsLetter(c) || c == '_')
                        {
                            sb.Append(c);
                            badChar = false;
                        }
                        else
                        {
                            if (!badChar)
                            {
                                switch (varType)
                                {
                                    case 0: sb = new(); break;
                                    case 1: ingredientTemplateName = sb.ToString(); sb = new(); break;
                                    case 2: ingredientTitleName = sb.ToString(); sb = new(); break;
                                }
                                varType++;
                            }
                            badChar = true;
                        }
                    }

                    ingredientTemplateNames.Add(ingredientTemplateName);
                    ingredientTitleNames.Add(ingredientTitleName);
                }

                if (data.Contains(chunkType) && data.Contains("crafting"))
                {
                    bool badChar = true;
                    StringBuilder sb = new();
                    string experimentalSubGroupTitle = "";
                    int varType = 0;

                    foreach (char c in data)
                    {
                        if (char.IsLetter(c) || c == '_')
                        {
                            sb.Append(c);
                            badChar = false;
                        }
                        else
                        {
                            if (!badChar)
                            {
                                switch (varType)
                                {
                                    case 0: sb = new(); break;
                                    case 1: sb = new(); break;
                                    case 2: experimentalSubGroupTitle = sb.ToString(); sb = new(); break;
                                }
                                varType++;
                            }
                            badChar = true;
                        }
                    }

                    experimentalSubGroupTitles.Add(experimentalSubGroupTitle);
                }
            }

            if (chunkType == "experiment")
            {
                if (data.Contains(chunkType))
                {
                    bool badChar = true;
                    StringBuilder sb = new();
                    string experimentalGroupTitle = "";
                    int varType = 0;

                    foreach (char c in data)
                    {
                        if (char.IsLetter(c) || c == '_')
                        {
                            sb.Append(c);
                            badChar = false;
                        }
                        else
                        {
                            if (!badChar)
                            {
                                switch (varType)
                                {
                                    case 0: sb = new(); break;
                                    case 1: sb = new(); break;
                                    case 2: experimentalGroupTitle = sb.ToString(); sb = new(); break;
                                }
                                varType++;
                            }
                            badChar = true;
                        }
                    }

                    if (string.IsNullOrEmpty(experimentalGroupTitle))
                    {
                        experimentalGroupTitle = "null";
                    }

                    experimentalGroupTitles.Add(experimentalGroupTitle);
                }
            }

            if (chunkType == "value")
            {
                if (data.Contains(chunkType))
                {
                    string data2 = data.Split("value")[1];

                    string value1 = $"{data2[3]}{data2[4]}{data2[5]}{data2[6]}";
                    string value2 = $"{data2[7]}{data2[8]}{data2[9]}{data2[10]}";

                    /*Console.WriteLine($"Hex value: {BitConverter.ToString(Encoding.ASCII.GetBytes(data2)).Replace("-", " ")}");
                    Console.WriteLine($"Hex value 1: {BitConverter.ToString(Encoding.ASCII.GetBytes(value1)).Replace("-", " ")}");
                    Console.WriteLine($"Hex value 2: {BitConverter.ToString(Encoding.ASCII.GetBytes(value2)).Replace("-", " ")}");*/

                    int min = Utils.StringToDecimal2(value1);
                    int max = Utils.StringToDecimal2(value2);

                    minValues.Add(min);
                    maxValues.Add(max);
                }
            }

            if (chunkType == "craftedSharedTemplate")
            {
                if (data.Contains(chunkType))
                {
                    string data2 = data.Split("craftedSharedTemplate")[1];
                    StringBuilder sb = new();

                    foreach (char c in data2)
                    {
                        if (char.IsLetter(c) || c == '/' || c == '_' || c == '.')
                        {
                            sb.Append(c);
                        }
                    }

                    _program.OnCraftedSharedTemplate(sb.ToString(), _iffFile);
                }
            }

            // TO DO:
            // craftedSharedTemplate

        }

        internal void GetChunkData(int chunkSize)
        {
            List<byte> data = new();

            for (int i = 0; i < chunkSize; i++)
            {
                data.Add(_br.ReadByte());
                bytesUntilEndOfFile -= 1;
            }

            string stringData = Encoding.ASCII.GetString(data.ToArray());

            CheckChunkFor("slots", stringData);
            CheckChunkFor("attributes", stringData);
            CheckChunkFor("name", stringData);
            CheckChunkFor("experiment", stringData);
            CheckChunkFor("value", stringData);
            CheckChunkFor("craftedSharedTemplate", stringData);
        }

        internal void FinishProcessing()
        {
            // _program.OnNextChunk(chunks, _iffFile);
            _program.OnIngredientTemplateName(ingredientTemplateNames, _iffFile);
            _program.OnIngredientTitleName(ingredientTitleNames, _iffFile);
            _program.OnExperimentalSubGroupTitle(experimentalSubGroupTitles, _iffFile);
            _program.OnExperimentalGroupTitle(experimentalGroupTitles, _iffFile);
            _program.OnMinValue(minValues, _iffFile);
            _program.OnMaxValue(maxValues, _iffFile);
        }
    }
}
