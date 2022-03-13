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
        [Ignore("Decode not implemented yet")]
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
        [Ignore("Decode not implemented yet")]
        public void DecodeReturnsNullWhenInputIsInvalid(string input)
        {
            Assert.IsNull(Bech32m.Decode(input));
        }


        [TestCase("BC1QW508D6QEJXTDG4Y5R3ZARVARY0C5XW7KV8F3T4", "0014751e76e8199196d454941c45d1b3a323f1433bd6")]
        [TestCase("tb1qrp33g0q5c5txsp9arysrx4k6zdkfs4nce4xj0gdcccefvpysxf3q0sl5k7",
            "00201863143c14c5166804bd19203356da136c985678cd4d27a1b8c6329604903262")]
        [TestCase("bc1pw508d6qejxtdg4y5r3zarvary0c5xw7kw508d6qejxtdg4y5r3zarvary0c5xw7kt5nd6y",
            "5128751e76e8199196d454941c45d1b3a323f1433bd6751e76e8199196d454941c45d1b3a323f1433bd6")]
        [TestCase("BC1SW50QGDZ25J", "6002751e")]
        [TestCase("bc1zw508d6qejxtdg4y5r3zarvaryvaxxpcs", "5210751e76e8199196d454941c45d1b3a323")]
        [TestCase("tb1qqqqqp399et2xygdj5xreqhjjvcmzhxw4aywxecjdzew6hylgvsesrxh6hy",
            "0020000000c4a5cad46221b2a187905e5266362b99d5e91c6ce24d165dab93e86433")]
        [TestCase("tb1pqqqqp399et2xygdj5xreqhjjvcmzhxw4aywxecjdzew6hylgvsesf3hn0c",
            "5120000000c4a5cad46221b2a187905e5266362b99d5e91c6ce24d165dab93e86433")]
        [TestCase("bc1p0xlxvlhemja6c4dqv22uapctqupfhlxm9h8z3k2e72q4k9hcz7vqzk5jj0",
            "512079be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798")]
        [Ignore("Decode not implemented yet")]
        public void DecodesAndEncodesValidStrings(string input, string output)
        {
            Assert.AreEqual(Bech32m.Decode(input), output);
            // Assert.AreEqual(Bech32m.Encode(output), input);
        }
        
        
    }
}
