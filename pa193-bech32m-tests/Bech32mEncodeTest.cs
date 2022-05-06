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
        public void EncodesNonEmptyHrpWithInput(string hrp, string input, string expected)
        {
            var (encodedString, errorMsg) = Bech32m.Encode(hrp, input);
            Assert.AreEqual(expected, encodedString);
            Assert.IsEmpty(errorMsg);
        }

        [Test]
        public void EncodedStringStartsWithHrp()
        {
            const string hrp = "abc";
            var (encodedString, errorMsg) = Bech32m.Encode(hrp, "12ef");
            Assert.That(encodedString, Does.StartWith(hrp));
            Assert.IsEmpty(errorMsg);
        }

        [Test]
        public void EncodedStringContainsSeparator()
        {
            var (encodedString, errorMsg) = Bech32m.Encode("abc", "12ef");
            Assert.That(encodedString, Does.Contain('1'));
            Assert.IsEmpty(errorMsg);
        }

        [Test]
        public void EncodesHrpContaining1()
        {
            var (encodedString, errorMsg) = Bech32m.Encode("a1b1c1", "12ef");
            Assert.AreEqual("a1b1c11zthsah97t8", encodedString);
            Assert.IsEmpty(errorMsg);
        }

        [Test]
        public void ReturnsNonZeroLengthStringWhenInputIsEmpty()
        {
            var (encodedString, errorMsg) = Bech32m.Encode("abc", "");
            CustomStringAssert.HasNonZeroLength(encodedString);
            Assert.IsEmpty(errorMsg);
        }

        /** ************** **/
        /** HRP Validation **/
        /** ************** **/
        [Test]
        public void ReturnsEmptyStringWhenHrpIsNull()
        {
            var (encodedString, errorMsg) = Bech32m.Encode(null, "12ef");
            Assert.IsEmpty(encodedString);
            Assert.AreEqual("hrp must not be empty", errorMsg);
        }

        [Test]
        public void ReturnsEmptyStringWhenHrpIsEmpty()
        {
            var (encodedString, errorMsg) = Bech32m.Encode("", "12ef");
            Assert.IsEmpty(encodedString);
            Assert.AreEqual("hrp must not be empty", errorMsg);
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
            var (encodedString, errorMsg) = Bech32m.Encode(hrp, "12ef");
            Assert.IsEmpty(encodedString);
            Assert.AreEqual("hrp contains invalid characters", errorMsg);
        }

        [TestCase(84)]
        [TestCase(1_000_000)]
        [TestCase(1_000_001)]
        public void ReturnsEmptyStringWhenHrpIsTooLong(int length)
        {
            var (encodedString, errorMsg) = Bech32m.Encode(new string('a', length), "12ef");
            Assert.IsEmpty(encodedString);
            Assert.AreEqual("hrp must not be longer than 83 characters", errorMsg);
        }

        /** **************** **/
        /** Input validation **/
        /** **************** **/
        [Test]
        public void ReturnsEmptyStringWhenInputIsNull()
        {
            var (encodedString, errorMsg) = Bech32m.Encode("abc", null);
            Assert.IsEmpty(encodedString);
            Assert.AreEqual("data are not in hex format", errorMsg);
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
            var (encodedString, errorMsg) = Bech32m.Encode("abc", input);
            Assert.IsEmpty(encodedString);
            Assert.AreEqual("data are not in hex format", errorMsg);
        }

        [TestCase('z', 1000)]
        [TestCase('z', 1_000_000)]
        [TestCase('a', 1001)]
        [TestCase('a', 1_000_001)]
        public void ReturnsEmptyStringWhenLongInputIsNotInHexFormat(char c, int length)
        {
            var (encodedString, errorMsg) = Bech32m.Encode("abc", new string(c, length));
            Assert.IsEmpty(encodedString);
            Assert.AreEqual("data are not in hex format", errorMsg);
        }

        /** ***************** **/
        /** Result validation **/
        /** ***************** **/
        [Test]
        public void ReturnsEmptyStringWhenResultWouldHaveMoreThan90Characters()
        {
            var (encodedString, errorMsg) = Bech32m.Encode(new string('a', 83), "00");
            Assert.IsEmpty(encodedString);
            Assert.AreEqual("Bech32m string is too long (92), maximum length is 90.", errorMsg);
        }

        [TestCase(1000, 810)]
        [TestCase(1_000_000, 800010)]
        public void ReturnsEmptyStringWhenResultWouldHaveMoreThan90CharactersForLongInput(int dataLength,
            int resultLength)
        {
            var (encodedString, errorMsg) = Bech32m.Encode("abc", new string('a', dataLength));
            Assert.IsEmpty(encodedString);
            Assert.AreEqual($"Bech32m string is too long ({resultLength}), maximum length is 90.", errorMsg);
        }
    }
}
