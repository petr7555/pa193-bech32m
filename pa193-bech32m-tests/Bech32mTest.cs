using NUnit.Framework;
using pa193_bech32m;

namespace pa193_bech32m_tests
{
    public class Bech32mTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("A1LQFN3A")]
        [TestCase("a1lqfn3a")]
        [TestCase("an83characterlonghumanreadablepartthatcontainsthetheexcludedcharactersbioandnumber11sg7hg6")]
        [TestCase("abcdef1l7aum6echk45nj3s0wdvt2fg8x9yrzpqzd3ryx")]
        [TestCase("11llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllludsr8")]
        [TestCase("split1checkupstagehandshakeupstreamerranterredcaperredlc445v")]
        [TestCase("?1v759aa")]
        public void DecodeReturnsNonZeroLengthStringWhenInputIsValid(string input)
        {
            var result = Bech32m.Decode(input);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [TestCase("\x20" + "1nwldj5")]
        [TestCase("\x7f" + "1axkwrx")]
        [TestCase("\x80" + "1axkwrx")]
        [TestCase("an84characterslonghumanreadablepartthatcontainsthetheexcludedcharactersbioandnumber11d6pts4")]
        [TestCase("qyrz8wqd2c9m")]
        [TestCase("1qyrz8wqd2c9m")]
        [TestCase("y1b0jsk6g")]
        [TestCase("lt1igcx5c0")]
        [TestCase("in1muywd")]
        [TestCase("mm1crxm3i")]
        [TestCase("au1s5cgom")]
        [TestCase("M1VUXWEZ")]
        [TestCase("16plkw9")]
        [TestCase("1p2gdwpf")]
        public void DecodeReturnsNullWhenInputIsInvalid(string input)
        {
            Assert.IsNull(Bech32m.Decode(input));
        }

        [TestCase("BC1QW508D6QEJXTDG4Y5R3ZARVARY0C5XW7KV8F3T4", "0014751e76e8199196d454941c45d1b3a323f1433bd6")]
        [TestCase("tb1qrp33g0q5c5txsp9arysrx4k6zdkfs4nce4xj0gdcccefvpysxf3q0sl5k7",
            "00201863143c14c5166804bd19203356da136c985678cd4d27a1b8c6329604903262")]
        public void DecodesAndEncodesValidStrings(string input, string output)
        {
            Assert.Equals(Bech32m.Decode(input), output);
            Assert.Equals(Bech32m.Encode(output), input);
        }
    }
}
