using NUnit.Framework;
using pa193_bech32m;

namespace pa193_bech32m_tests
{
    public class Bech32mEncodeTest
    {
        [TestCase("abc", "", "abc1k7c8sc")]
        [TestCase("abc", "00", "abc1qqlu4nty")]
        [TestCase("abc", "01", "abc1qyskh4y7")]
        [TestCase("abc", "0000", "abc1qqqqdwreek")]
        [TestCase("abc", "0001", "abc1qqqsc0tpv9")]
        [TestCase("abc", "12ef", "abc1zths84d6rg")]
        [TestCase("abc", "12Ef", "abc1zths84d6rg")]
        [TestCase("abc", "12EF", "abc1zths84d6rg")]
        [TestCase("A", "12ef", "A1zths6wsktz")]
        [TestCase("ABC", "12ef", "ABC1zthshfzj6z")]
        [TestCase("abCd", "12ef", "abCd1zths7wfa9m")]
        [TestCase("!\"#~", "12ef", "!\"#~1zths5pyw4y")]
        [TestCase("bech32", "f301df5dad7c67c25008c328", "bech3217vqa7hdd03nuy5qgcv5qjhzlhg")]
        [TestCase("bech32", "6293a977e46a21b83bb66c73214e6802623f92569ab5666b40",
            "bech321v2f6jalydgsmswakd3ejznngqf3rlyjkn26kv66qgj9fwl")]
        [TestCase("1234567891234567891234567891234567891234567", "6293a977e46a21b83bb66c73214e6802623f92569ab5666b40",
            "12345678912345678912345678912345678912345671v2f6jalydgsmswakd3ejznngqf3rlyjkn26kv66qs0qzg3")]
        [TestCase("11111111112222222222333333333344444444445555555555666666666677777777778888888888999", "",
            "1111111111222222222233333333334444444444555555555566666666667777777777888888888899919h6dca")]
        [TestCase("0", "", "01nn8ez9")]
        [TestCase("0",
            "1111111111222222222233333333334444444444555555555566666666667777777777888888888899999999990000000000",
            "01zyg3zyg3yg3zyg3zxvenxveng3zyg3zy24242424venxvenxwamhwamh3zyg3zygnxvenxveqqqqqqqqx7f72k")]
        [TestCase("1", "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffc0", "11llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllqq4dt6ek")]
        public void EncodesNonEmptyHrpWithInput(string hrp, string input, string expected)
        {
            Assert.AreEqual(expected, Bech32m.Encode(hrp, input));
        }

        [Test]
        public void EncodedStringStartsWithHrp()
        {
            const string hrp = "abc";
            Assert.That(Bech32m.Encode(hrp, "12ef"), Does.StartWith(hrp));
        }

        [Test]
        public void EncodedStringContainsSeparator()
        {
            Assert.That(Bech32m.Encode("abc", "12ef"), Does.Contain('1'));
        }

        [Test]
        public void EncodesHrpContaining1()
        {
            Assert.AreEqual("a1b1c11zthsah97t8", Bech32m.Encode("a1b1c1", "12ef"));
        }

        [Test]
        public void ReturnsNonZeroLengthStringWhenInputIsEmpty()
        {
            StringAssert.HasNonZeroLength(Bech32m.Encode("abc", ""));
        }

        [TestCase(1000)]
        [TestCase(1_000_000)]
        public void ReturnsNonZeroLengthStringForLongInput(int length)
        {
            StringAssert.HasNonZeroLength(Bech32m.Encode("abc", new string('a', length)));
        }

        /** ************** **/
        /** HRP Validation **/
        /** ************** **/
        [Test]
        public void ReturnsEmptyStringWhenHrpIsNull()
        {
            Assert.IsEmpty(Bech32m.Encode(null, "12ef"));
        }

        [Test]
        public void ReturnsEmptyStringWhenHrpIsEmpty()
        {
            Assert.IsEmpty(Bech32m.Encode("", "12ef"));
        }

        [TestCase("\x20")]
        [TestCase(" ")]
        [TestCase("\x20" + "abc")]
        [TestCase("ab cd")]
        [TestCase("\x7f")]
        [TestCase("a\x7f" + "b")]
        [TestCase("5\x80")]
        [TestCase("\x00")]
        [TestCase("\x14")]
        public void ReturnsEmptyStringWhenHrpContainsInvalidCharacters(string hrp)
        {
            Assert.IsEmpty(Bech32m.Encode(hrp, "12ef"));
        }

        [TestCase(84)]
        [TestCase(1_000_000)]
        [TestCase(1_000_001)]
        public void ReturnsEmptyStringWhenHrpIsTooLong(int length)
        {
            Assert.IsEmpty(Bech32m.Encode(new string('a', length), "12ef"));
        }

        /** **************** **/
        /** Input validation **/
        /** **************** **/
        [Test]
        public void ReturnsEmptyStringWhenInputIsNull()
        {
            Assert.IsEmpty(Bech32m.Encode("abc", null));
        }

        [TestCase("0")]
        [TestCase("1")]
        [TestCase("123")]
        [TestCase("z")]
        [TestCase("xx")]
        [TestCase("abz")]
        [TestCase(" ")]
        [TestCase("\x7f\x7f")]
        [TestCase("a\x7f" + "bc")]
        [TestCase("5\x80")]
        [TestCase("\x00")]
        [TestCase("\x14")]
        public void ReturnsEmptyStringWhenInputIsNotInHexFormat(string input)
        {
            Assert.IsEmpty(Bech32m.Encode("abc", input));
        }

        [TestCase('z', 1000)]
        [TestCase('z', 1_000_000)]
        [TestCase('a', 1001)]
        [TestCase('a', 1_000_001)]
        public void ReturnsEmptyStringWhenLongInputIsNotInHexFormat(char c, int length)
        {
            Assert.IsEmpty(Bech32m.Encode("abc", new string(c, length)));
        }
    }
}
