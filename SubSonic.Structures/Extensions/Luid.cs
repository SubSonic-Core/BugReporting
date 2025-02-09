using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SubSonic
{
    public static partial class Extensions
    {
#if !NET8_0_OR_GREATER
        public static bool IsHexDigitLower(this char value)
        {
            return value.IsHexDigit(true);
        }

        public static bool IsHexDigit(this char value, bool lowercase)
        {
            return (value >= '0' && value <= '9') || (value >= 'a' && value <= 'f') || (!lowercase && value >= 'A' && value <= 'F');
        }
#endif

        public static string ToHexLowerString(this ReadOnlySpan<byte> hexBytes)
        {
            return BitConverter.ToString(hexBytes.ToArray()).Replace("-", "").ToLower();
        }

        public static void ToSpanFromHexChars(this ReadOnlySpan<char> hexCharString, Span<byte> bytes)
        {
            Debug.Assert(bytes.Length * 2 == hexCharString.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = HexByteFromChars(hexCharString[i * 2], hexCharString[i * 2 + 1]);
            }
        }

        private static byte HexByteFromChars(char chi, char clo)
        {
            int hi = Convert.ToInt32(new string(chi, 1), 16);
            int lo = Convert.ToInt32(new string(clo, 1), 16);

            if((hi | lo) == 0xFF)
            {
                throw new ArgumentOutOfRangeException("chi|clo", $"0x{chi}{clo} is out of range");
            }

            return (byte) ((hi << 4) | lo);
        }
    }
}
