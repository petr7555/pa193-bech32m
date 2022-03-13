using System;
using System.Collections.Generic;
using System.Linq;

namespace pa193_bech32m
{
    public class Bech32m
    {
        private const string Charset = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";
        private static readonly int[] Generator = {0x3b6a57b2, 0x26508e6d, 0x1ea119fa, 0x3d4233dd, 0x2a1462b3};

        private static List<int> HrpExpand(string hrp)
        {
            var result = hrp.Select(c => c >> 5).ToList();
            result.Add(0);
            result.AddRange(hrp.Select(c => c & 31));
            return result;
        }

        private static int Polymod(List<int> values)
        {
            var chk = 1;
            foreach (var val in values)
            {
                var top = chk >> 25;
                chk = (chk & 0x1ffffff) << 5 ^ val;
                for (var i = 0; i < 5; ++i)
                {
                    if (((top >> i) & 1) == 1)
                    {
                        chk ^= Generator[i];
                    }
                }
            }

            return chk;
        }

        private static List<int> CreateChecksum(string hrp, List<int> data)
        {
            var values = HrpExpand(hrp).Concat(data).Concat(new[] {0, 0, 0, 0, 0, 0}).ToList();
            var polymod = Polymod(values) ^ 0x2bc830a3;
            var result = new List<int>();
            for (var p = 0; p < 6; ++p)
            {
                result.Add((polymod >> 5 * (5 - p)) & 31);
            }

            return result;
        }

        private static List<int> ConvertToBase32(string input)
        {
            var inputInBinary = string.Join(string.Empty, input.Select(
                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                )
            );

            inputInBinary = inputInBinary.PadRight(Convert.ToInt32(5 * Math.Ceiling(inputInBinary.Length / 5.0)), '0');
            var result = new List<int>();
            for (var i = 0; i < inputInBinary.Length; i += 5)
            {
                var fiveBits = inputInBinary.Substring(i, 5);
                var numInDec = Convert.ToInt32(fiveBits, 2);
                result.Add(numInDec);
            }

            return result;
        }

        private static bool IsValidHrp(string hrp)
        {
            if (string.IsNullOrEmpty(hrp) || hrp.Length > 83)
            {
                return false;
            }

            if (hrp.Any(c => c < 33 || c > 126))
            {
                return false;
            }

            return true;
        }

        private static bool IsValidInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }


            return true;
        }

        public static string Encode(string hrp, string input)
        {
            if (!IsValidHrp(hrp) || !IsValidInput(input))
            {
                return "";
            }

            var data = ConvertToBase32(input);
            var checksum = CreateChecksum(hrp, data);
            var combined = data.Concat(checksum);
            var encoded = string.Join(string.Empty, combined.Select(i => Charset[i]));
            var result = hrp + "1" + encoded;
            return result;
        }

        public static string Decode(string input)
        {
            return "";
        }
    }
}
