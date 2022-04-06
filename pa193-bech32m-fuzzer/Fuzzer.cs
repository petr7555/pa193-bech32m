using pa193_bech32m;
using SharpFuzz;

namespace pa193_bech32m_fuzzer
{
    public static class Bech32mFuzzer
    {
        public static void FuzzHrp(string input)
        {
            Bech32m.Encode(input, "12ef");
        }

        public static void FuzzData(string input)
        {
            Bech32m.Encode("abc", input);
        }

        public static void FuzzHrpAndData(string input)
        {
            var hrp = input[..(input.Length / 2)];
            var data = input[(input.Length / 2)..];
            Bech32m.Encode(hrp, data);
        }

        public static void Main()
        {
            /* Choose what to fuzz */
            // Fuzzer.Run(FuzzData);
            // Fuzzer.Run(FuzzHrp);
            Fuzzer.Run(FuzzHrpAndData);
        }
    }
}
