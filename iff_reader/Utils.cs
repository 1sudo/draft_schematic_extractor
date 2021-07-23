using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iff_reader
{
    class Utils
    {
        internal static int SwapEndianness(int value)
        {
            var b1 = (value >> 0) & 0xff;
            var b2 = (value >> 8) & 0xff;
            var b3 = (value >> 16) & 0xff;
            var b4 = (value >> 24) & 0xff;

            return b1 << 24 | b2 << 16 | b3 << 8 | b4 << 0;
        }

        internal static int StringToDecimal(string value)
        {
            try
            {
                if (value.Length == 4)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(value);
                    StringBuilder sb = new();
                    int val = 0;

                    foreach (byte b in bytes)
                    {
                        val += Convert.ToInt32(b);
                    }

                    return val;
                }
            }
            catch
            {
                Console.WriteLine($"Incorrect string size: {value.Length}");
            }

            return 0;
        }

        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        internal static int StringToDecimal2(string value)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(value);
            return BitConverter.ToInt32(bytes, 0);
        }



        public static string Reverse(string input)
        {
            char[] chars = input.ToCharArray();
            Array.Reverse(chars);
            return new(chars);
        }
    }
}
