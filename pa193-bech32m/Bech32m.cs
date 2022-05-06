using System;
using System.Collections.Generic;
using System.Linq;

namespace pa193_bech32m
{
    public static class Bech32m
    {
        private const string Charset = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";
        private static readonly int[] Generator = {0x3b6a57b2, 0x26508e6d, 0x1ea119fa, 0x3d4233dd, 0x2a1462b3};
        private const int HrpMaxLength = 83;
        private const int Bech32MMaxLength = 90;

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

        private static string PadRightWithZerosToMultipleOf(string inputInBinary, int multiple)
        {
            return inputInBinary.PadRight(
                Convert.ToInt32(multiple * Math.Ceiling(inputInBinary.Length / (float) multiple)), '0');
        }

        private static List<int> ConvertToBase32(string input)
        {
            var inputInBinary = string.Join(string.Empty, input.Select(
                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                )
            );

            inputInBinary = PadRightWithZerosToMultipleOf(inputInBinary, 5);
            var result = new List<int>();
            for (var i = 0; i < inputInBinary.Length; i += 5)
            {
                var fiveBits = inputInBinary.Substring(i, 5);
                var numInDec = Convert.ToInt32(fiveBits, 2);
                result.Add(numInDec);
            }

            return result;
        }

        public static (bool isValid, string errorMsg) ValidateHrp(string hrp)
        {
            if (string.IsNullOrEmpty(hrp))
            {
                return (false, "hrp must not be empty");
            }

            if (hrp.Length > HrpMaxLength)
            {
                return (false, $"hrp must not be longer than {HrpMaxLength} characters");
            }

            if (hrp.Any(c => c < 33 || c > 126))
            {
                return (false, "hrp contains invalid characters");
            }

            return (true, "");
        }

        public static bool IsValidHexInput(string input)
        {
            if (input is null)
            {
                return false;
            }

            if (input.Length % 2 == 1)
            {
                return false;
            }

            if (input.Any(c => (c < '0' || c > '9') &&
                               (c < 'a' || c > 'f') &&
                               (c < 'A' || c > 'F')))
            {
                return false;
            }

            return true;
        }

        private static (bool isValid, string errorMsg) ValidateResult(string encodedString)
        {
            if (encodedString.Length > Bech32MMaxLength)
            {
                return (false,
                    $"Bech32m string is too long ({encodedString.Length}), maximum length is {Bech32MMaxLength}.");
            }

            return (true, "");
        }

        /// <summary>
        /// Encodes hrp and input data into Bech32m string.
        /// </summary>
        /// <param name="hrp">Human-readable part - string consisting of 1 to 83 US-ASCII characters,
        /// with each character having a value in the range [33-126]</param>
        /// <param name="input">String of any length (including zero length) in hexadecimal format.</param>
        /// 
        /// <returns>Bech32m string and error message (nonempty when error occurred)</returns>
        public static (string encodedString, string errorMsg) Encode(string hrp, string input)
        {
            var (isValidHrp, errorMsgHrp) = ValidateHrp(hrp);
            if (!isValidHrp)
            {
                return ("", errorMsgHrp);
            }

            if (!IsValidHexInput(input))
            {
                return ("", "data are not in hex format");
            }

            var data = ConvertToBase32(input);
            var checksum = CreateChecksum(hrp, data);
            var combined = data.Concat(checksum);
            var encoded = string.Join(string.Empty, combined.Select(i => Charset[i]));
            var result = hrp + "1" + encoded;

            var (isValidResult, errorMsgResult) = ValidateResult(result);
            if (!isValidResult)
            {
                return ("", errorMsgResult);
            }

            return (result, "");
        }

        public static (string, string) Decode(string input)
        {
            return ("", "");
        }
    }
}
