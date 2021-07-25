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
        readonly Program _program;
        readonly IFFFile _iffFile;
        readonly List<string> ingredientTemplateNames;
        readonly List<string> ingredientTitleNames;
        readonly List<string> experimentalSubGroupTitles;
        readonly List<string> experimentalGroupTitles;
        readonly List<int> minValues;
        readonly List<int> maxValues;
        bool isXp = false;
        bool isComplexity = false;

        internal Chunk(BinaryReader br, IFFFile iffFile)
        {
            _br = br;
            _program = new();
            _iffFile = iffFile;

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

        internal void CheckChunkFor(string chunkType, string data, int type = 2)
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

                    if (experimentalSubGroupTitle == "xp")
                    {
                        isXp = true;
                        experimentalSubGroupTitles.Add("null");
                    }   
                    else if (experimentalSubGroupTitle == "complexity")
                    {
                        isComplexity = true;
                        experimentalSubGroupTitles.Add("null");
                    }
                    else 
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

                    if (type == 0)
                        experimentalGroupTitles.Add("null");
                    else if (type == 1)
                        experimentalGroupTitles.Add("null");
                    else
                        experimentalGroupTitles.Add(experimentalGroupTitle);
                }
            }

            if (chunkType == "76616C7565000320")
            {
                if (data.Contains(chunkType))
                {
                    data = data.Split(chunkType)[1];

                    byte[] v1 = Utils.StringToByteArrayFastest($"{data[0]}{data[1]}{data[2]}{data[3]}{data[4]}{data[5]}{data[6]}{data[7]}");
                    byte[] v2 = Utils.StringToByteArrayFastest($"{data[8]}{data[9]}{data[10]}{data[11]}{data[12]}{data[13]}{data[14]}{data[15]}");

                    if (type == 0)
                    {
                        minValues.Add(0);
                        maxValues.Add(0);
                        _program.OnXp(BitConverter.ToInt32(v1, 0), _iffFile);
                        isXp = false;
                    }
                    else if (type == 1)
                    {
                        minValues.Add(0);
                        maxValues.Add(0);
                        _program.OnComplexity(BitConverter.ToInt32(v1, 0), _iffFile);
                        isComplexity = false;
                    }
                    else
                    {
                        minValues.Add(BitConverter.ToInt32(v1, 0));
                        maxValues.Add(BitConverter.ToInt32(v2, 0));
                    }
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
        }

        internal void GetChunkData(int chunkSize)
        {
            List<byte> data = new();

            for (int i = 0; i < chunkSize; i++)
            {
                data.Add(_br.ReadByte());
                bytesUntilEndOfFile -= 1;
            }

            string hexData = BitConverter.ToString(data.ToArray()).Replace("-", "");
            string stringData = Encoding.ASCII.GetString(data.ToArray());

            CheckChunkFor("slots", stringData);
            CheckChunkFor("attributes", stringData);
            CheckChunkFor("name", stringData);

            if (isXp)
                CheckChunkFor("experiment", stringData, 0);
            else if (isComplexity)
                CheckChunkFor("experiment", stringData, 1);
            else
                CheckChunkFor("experiment", stringData);

            CheckChunkFor("craftedSharedTemplate", stringData);

            if (isXp)
                CheckChunkFor("76616C7565000320", hexData, 0);
            else if (isComplexity)
                CheckChunkFor("76616C7565000320", hexData, 1);
            else
                CheckChunkFor("76616C7565000320", hexData);
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
