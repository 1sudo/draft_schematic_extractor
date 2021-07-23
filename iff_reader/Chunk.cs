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

        internal static Action<string> OnNextChunk;
        internal static Action<int> OnFileSize;
        internal static Action<int> OnSlots;
        internal static Action<int> OnAttributes;
        internal static Action<string> OnIngredientTemplateName;
        internal static Action<string> OnIngredientTitleName;
        internal static Action<string> OnExperimentalSubGroupTitle;
        internal static Action<string> OnExperimentalGroupTitle;
        internal static Action<int> OnMinValue;
        internal static Action<int> OnMaxValue;

        internal Chunk(BinaryReader br)
        {
            _br = br;
        }

        internal string GetNextChunk()
        {
            if (bytesUntilEndOfFile is not null) bytesUntilEndOfFile -= 4;
            var value = _br.ReadBytes(4);
            return Encoding.ASCII.GetString(value);
        }

        internal int GetFileSize()
        {
            var value = Utils.SwapEndianness(_br.ReadInt32());
            if (bytesUntilEndOfFile is null) bytesUntilEndOfFile = value;
            OnFileSize?.Invoke(value);
            return value;
        }

        internal int GetChunkSize()
        {
            int value = Utils.SwapEndianness(_br.ReadInt32());
            bytesUntilEndOfFile -= 4;
            return value;
        }

        internal static void CheckChunkFor(string chunkType, string data)
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
                        OnSlots?.Invoke(slots);
                    }
                    if (chunkType == "attributes")
                    {
                        int attributes = Utils.StringToDecimal(sb.ToString().Substring(2, 4));
                        OnAttributes?.Invoke(attributes);
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

                    OnIngredientTemplateName?.Invoke(ingredientTemplateName);
                    OnIngredientTitleName?.Invoke(ingredientTitleName);
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

                    OnExperimentalSubGroupTitle?.Invoke(experimentalSubGroupTitle);
                }
            }

            if (chunkType == "experiment")
            {
                if (data.Contains(chunkType) && data.Contains("crafting"))
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

                    OnExperimentalGroupTitle?.Invoke(experimentalGroupTitle);
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

                    OnMinValue?.Invoke(min);
                    OnMaxValue?.Invoke(max);
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
        }
    }
}
