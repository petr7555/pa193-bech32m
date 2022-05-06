using NUnit.Framework;
using pa193_bech32m;

namespace pa193_bech32m_tests
{
    [Ignore("Decode not implemented yet")]
    public class Bech32mDecodeTest
    {
        [TestCase("abcdef140x77khk82w", "abcdef", "abcdef")]
        [TestCase("test1wejkxar0wg64ekuu", "test", "766563746f72")]
        [TestCase("A1LQFN3A", "a", "")]
        [TestCase("a1lqfn3a", "a", "")]
        [TestCase("an83characterlonghumanreadablepartthatcontainsthetheexcludedcharactersbioandnumber11sg7hg6",
            "an83characterlonghumanreadablepartthatcontainsthetheexcludedcharactersbioandnumber1", "")]
        [TestCase("abcdef1l7aum6echk45nj3s0wdvt2fg8x9yrzpqzd3ryx", "abcdef",
            "ffbbcdeb38bdab49ca307b9ac5a928398a418820")]
        [TestCase("split1checkupstagehandshakeupstreamerranterredcaperredlc445v", "split",
            "c5f38b70305f519bf66d85fb6cf03058f3dde463ecd7918f2dc743918f2d")]
        [TestCase("?1v759aa", "?", "")]
        // Binary input of 410 '1's gets padded to 410 '1's and 6 '0's to be divisible by 8 (and therefore hex-encodable).
        // Which results in input ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffc0,
        // which after encoding (with hrp = '1') is 11llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllqq4dt6ek
        // (which has length > 90 and is therefore invalid).
        // If we could pass binary input directly to encode() as 410 '1's, it would be split by 5 '1's into 82 '31's, resulting
        // in 11llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllludsr8.
        // In reference python implementation: `bech32_encode("1", [0b0001_1111] * (410 // 5), Encoding.BECH32M)`.
        // By decoding 11llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllludsr8 we get again 82 '31's,
        // which is padded by 6 '0's to be representable by hex-string ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffc0.  
        // In reference python implementation: `bech32_decode("11llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllludsr8")`.
        [TestCase("11llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllludsr8", "1",
            "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffc0")]
        public void DecodesValidInput(string input, string expectedHrp, string expectedData)
        {
            var (hrp, data) = Bech32m.Decode(input);
            Assert.AreEqual(expectedHrp, hrp);
            Assert.AreEqual(expectedData, data);
            Assert.AreEqual(input, Bech32m.Encode(hrp, data));
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
        [TestCase("tc1p0xlxvlhemja6c4dqv22uapctqupfhlxm9h8z3k2e72q4k9hcz7vq5zuyut")]
        [TestCase("bc1p0xlxvlhemja6c4dqv22uapctqupfhlxm9h8z3k2e72q4k9hcz7vqh2y7hd")]
        [TestCase("tb1z0xlxvlhemja6c4dqv22uapctqupfhlxm9h8z3k2e72q4k9hcz7vqglt7rf")]
        [TestCase("BC1S0XLXVLHEMJA6C4DQV22UAPCTQUPFHLXM9H8Z3K2E72Q4K9HCZ7VQ54WELL")]
        [TestCase("bc1qw508d6qejxtdg4y5r3zarvary0c5xw7kemeawh")]
        [TestCase("tb1q0xlxvlhemja6c4dqv22uapctqupfhlxm9h8z3k2e72q4k9hcz7vq24jc47")]
        [TestCase("bc1p38j9r5y49hruaue7wxjce0updqjuyyx0kh56v8s25huc6995vvpql3jow4")]
        [TestCase("BC130XLXVLHEMJA6C4DQV22UAPCTQUPFHLXM9H8Z3K2E72Q4K9HCZ7VQ7ZWS8R")]
        [TestCase("bc1pw5dgrnzv")]
        [TestCase("bc1p0xlxvlhemja6c4dqv22uapctqupfhlxm9h8z3k2e72q4k9hcz7v8n0nx0muaewav253zgeav")]
        [TestCase("BC1QR508D6QEJXTDG4Y5R3ZARVARYV98GJ9P")]
        [TestCase("tb1p0xlxvlhemja6c4dqv22uapctqupfhlxm9h8z3k2e72q4k9hcz7vq47Zagq")]
        [TestCase("bc1p0xlxvlhemja6c4dqv22uapctqupfhlxm9h8z3k2e72q4k9hcz7v07qwwzcrf")]
        [TestCase("tb1p0xlxvlhemja6c4dqv22uapctqupfhlxm9h8z3k2e72q4k9hcz7vpggkg4j")]
        [TestCase("bc1gmk9yu")]
        public void DecodeReturnsEmptyStringsWhenInputIsInvalid(string input)
        {
            var (hrp, data) = Bech32m.Decode(input);
            Assert.IsEmpty(hrp);
            Assert.IsEmpty(data);
        }
    }
}
