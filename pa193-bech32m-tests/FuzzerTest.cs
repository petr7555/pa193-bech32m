using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using pa193_bech32m_fuzzer;

namespace pa193_bech32m_tests
{
    public class FuzzerTest
    {
        [TestCase("")]
        [TestCase("a")]
        [TestCase("ab")]
        [TestCase("abc")]
        [TestCase("abcd")]
        public void CanRunFuzzFunctionsWithoutException(string input)
        {
            Bech32mFuzzer.FuzzHrp(input);
            Bech32mFuzzer.FuzzData(input);
            Bech32mFuzzer.FuzzHrpAndData(input);
            Bech32mFuzzer.FuzzCliHrpAndData(input);
            Bech32mFuzzer.FuzzCliWithHexInputFile(input);
            Bech32mFuzzer.FuzzCliWithBase64InputFile(input);
            Bech32mFuzzer.FuzzCliWithBinaryInputFile(input);
        }

        private static (string, int) Run(string[] args)
        {
            var outMemoryStream = new MemoryStream();
            Console.SetOut(new StreamWriter(outMemoryStream) {AutoFlush = true});

            var exitCode = Bech32mFuzzer.Main(args);
            return (Encoding.Default.GetString(outMemoryStream.ToArray()), exitCode);
        }

        [Test]
        public void PrintsErrorAndReturnsOneWhenFuzzTestNameIsNotGivenAsArgument()
        {
            Assert.AreEqual(("Pass fuzz test name as the first argument.\n", 1), Run(Array.Empty<string>()));
        }

        [Test]
        public void PrintsErrorAndReturnsOneWhenFuzzTestGivenAsArgumentDoesNotExist()
        {
            const string fuzzTestName = "nonexistent";
            Assert.AreEqual(($"Fuzz test {fuzzTestName} does not exist.\n", 1), Run(new[] {fuzzTestName}));
        }
    }
}
