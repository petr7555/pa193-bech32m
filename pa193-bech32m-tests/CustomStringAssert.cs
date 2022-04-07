using NUnit.Framework;

namespace pa193_bech32m_tests
{
    public static class CustomStringAssert
    {
        public static void HasNonZeroLength(string s)
        {
            Assert.IsNotNull(s);
            Assert.IsNotEmpty(s);
        }
    }
}
