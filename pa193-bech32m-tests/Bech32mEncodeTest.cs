using NUnit.Framework;
using pa193_bech32m;

namespace pa193_bech32m_tests
{
    public class Bech32mEncodeTest
    {
        [TestCase("abc", "12ef", "abc1zths84d6rg")]
        [TestCase("!\"#~", "12ef", "!\"#~1zths5pyw4y")]
        [TestCase("bech32", "f301df5dad7c67c25008c328", "bech3217vqa7hdd03nuy5qgcv5qjhzlhg")]
        [TestCase("bech32", "6293a977e46a21b83bb66c73214e6802623f92569ab5666b40", "bech321v2f6jalydgsmswakd3ejznngqf3rlyjkn26kv66qgj9fwl")]
        public void EncodesNonEmptyHrpWithNonEmptyData(string hrp, string input, string expected)
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

        [Test]
        public void ReturnsEmptyStringWhenInputIsEmpty()
        {
            Assert.IsEmpty(Bech32m.Encode("abc", ""));
        }
    }
}
