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
        }
    }
}
