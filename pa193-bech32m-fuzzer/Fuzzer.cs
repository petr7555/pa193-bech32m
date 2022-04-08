﻿using System;
using pa193_bech32m;
using SharpFuzz;

namespace pa193_bech32m_fuzzer
{
    public static class Bech32mFuzzer
    {
        private static (string, string) SplitInHalf(string input) =>
            (input[..(input.Length / 2)], input[(input.Length / 2)..]);

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
            var (hrp, data) = SplitInHalf(input);
            Bech32m.Encode(hrp, data);
        }

        public static void FuzzCliHrpAndData(string input)
        {
            var (hrp, data) = SplitInHalf(input);
            Program.Main(new[] {"encode", "--hrp", hrp, data});
        }

        public static int Main(string[] args)
        {
            const int exitSuccess = 0;
            const int exitFailure = 1;

            if (args.Length < 1)
            {
                Console.WriteLine("Pass fuzz test name as the first argument.");
                return exitFailure;
            }

            var fuzzTestName = args[0];
            var fuzzTest = typeof(Bech32mFuzzer).GetMethod(fuzzTestName);
            if (fuzzTest is null)
            {
                Console.WriteLine($"Fuzz test {fuzzTestName} does not exist.");
                return exitFailure;
            }

            Fuzzer.Run(input => { fuzzTest.Invoke(null, new object[] {(string) input}); });

            return exitSuccess;
        }
    }
}
