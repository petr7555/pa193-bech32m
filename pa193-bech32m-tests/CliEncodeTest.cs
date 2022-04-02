using System.IO;
using NUnit.Framework;
using static pa193_bech32m_tests.CliTest;

namespace pa193_bech32m_tests
{
    public class CliEncodeTest
    {
        private const string EncodeUsage = @"Usage: bech32m encode [options] <data>

encode hrp and data into Bech32m string

Arguments:
  data                       data to encode. If neither this argument nor ""--input"" is given, data are read from stdin.

Options:
  --hrp <hrp>                human-readable part. Consists of 1–83 ASCII characters in range [33, 126].
  -f, --format <format>      format of input and output (choices: ""hex"", ""base64"", ""binary"", default: ""hex"")
  -i, --input <inputfile>    input file with data
  -o, --output <outputfile>  output file where result will be saved. If not present, result is printed to stdout.
  -h, --help                 display help for command
";

        private const string TestInputFile = "test_input_file";
        private const string TestOutputFile = "test_output_file";

        [OneTimeSetUp]
        public void BeforeAll()
        {
            File.WriteAllText(TestInputFile, "12ef");
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
            File.Delete(TestInputFile);
        }

        [SetUp]
        public void BeforeEach()
        {
            Assert.False(File.Exists(TestOutputFile));
        }

        [TearDown]
        public void AfterEach()
        {
            File.Delete(TestOutputFile);
        }

        [TestCase("-h")]
        [TestCase("--help")]
        public void PrintsUsageOfEncodeCommandAndExitsWithZeroWhenHelpFlagPassedAfterEncode(string helpFlag)
        {
            Assert.AreEqual((EncodeUsage, 0), Run("encode", helpFlag));
        }

        [TestCase("-h")]
        [TestCase("--help")]
        public void PrintsUsageOfEncodeCommandAndExitsWithZeroWhenHelpFlagPassedAnywhereAfterEncode(string helpFlag)
        {
            Assert.AreEqual((EncodeUsage, 0), Run("encode", "foo", "--bar", helpFlag, "baz"));
        }

        [Test]
        public void PrintsUsageOfEncodeCommandAndExitsWithZeroWhenEncodePassedAsArgumentToHelp()
        {
            Assert.AreEqual((EncodeUsage, 0), Run("help", "encode"));
        }

        [Test]
        public void PrintsRequiredOptionNotSpecifiedAndExitsWithOneWhenCalledWithoutHrp()
        {
            Assert.AreEqual(("error: required option '--hrp <hrp>' not specified\n", 1), Run("encode"));
        }

        [Test]
        public void PrintsOptionArgumentMissingAndExitsWithOneWhenCalledWithHrpOptionWithoutHrpArgument()
        {
            Assert.AreEqual(("error: option '--hrp <hrp>' argument missing\n", 1), Run("encode", "--hrp"));
        }

        [Test]
        public void OptionLikeArgumentIsConsideredValidArgumentForHrpOption()
        {
            Assert.AreEqual(("--format1zths4ad7ku\n", 0), Run("encode", "--hrp", "--format", "12ef"));
            Assert.AreEqual(("-f1zthsw2s0yj\n", 0), Run("encode", "--hrp", "-f", "12ef"));
        }

        [Test]
        public void
            PrintsEncodedStringAndExitsWithZeroWhenCalledWithValidHrpAndInputFileAndWithoutData()
        {
            Assert.AreEqual(("abc1zths84d6rg\n", 0), Run("encode", "--hrp", "abc", "--input", TestInputFile));
        }

        [Test]
        public void IgnoresOtherArgumentsWhenAllNecessaryParametersAreProvided()
        {
            Assert.AreEqual(("abc1zths84d6rg\n", 0), Run("encode", "--hrp", "abc", "12ef", "xy", "ab"));
        }

        [TestCase("12ef")]
        [TestCase("xy")]
        public void
            InputParamTakesPrecedenceBeforePassedDataWhichCanEvenBeInvalid(string data)
        {
            Assert.AreEqual(("abc1zths84d6rg\n", 0),
                Run("encode", "--hrp", "abc", data, "--input", TestInputFile));
        }

        [Test]
        public void
            PrintsEncodedStringAndExitsWithZeroWhenCalledWithValidHrpAndWithHexDataBeforeOrAfterHrpOption()
        {
            Assert.AreEqual(("abc1zths84d6rg\n", 0), Run("encode", "12ef", "--hrp", "abc"));
            Assert.AreEqual(("abc1zths84d6rg\n", 0), Run("encode", "--hrp", "abc", "12ef"));
        }

        [Test]
        public void PrintsEncodedStringAndExitsWithZeroWhenCalledWithValidHrpAndDataAreBetweenOptions()
        {
            Assert.AreEqual(("abc1zths84d6rg\n", 0), Run("encode", "--format", "hex", "12ef", "--hrp", "abc"));
        }

        [Test]
        public void PrintsUnknownOptionAndExitsWithOneWhenCalledWithUnknownOption()
        {
            Assert.AreEqual(("error: unknown option '--bar'\n", 1), Run("encode", "--hrp", "abc", "12ef", "--bar"));
            Assert.AreEqual(("error: unknown option '-b'\n", 1), Run("encode", "--hrp", "abc", "12ef", "-b"));
        }

        [Test]
        public void PrintsInvalidArgumentAndExitsWithOneWhenCalledWithFormatOptionAndInvalidFormatArgument()
        {
            Assert.AreEqual(
                ("error: option '-f, --format <format>' argument 'base10' is invalid. Allowed choices are hex, base64, binary.\n",
                    1), Run("encode", "--format", "base10"));
        }

        /** ************** **/
        /** HRP validation **/
        /** ************** **/
        [Test]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithEmptyHrp()
        {
            Assert.AreEqual(("error: hrp must not be empty\n", 1), Run("encode", "--hrp", "", "12ef"));
        }

        [TestCase(84)]
        [TestCase(1_000_000)]
        [TestCase(1_000_001)]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithTooLongHrp(int hrpLength)
        {
            Assert.AreEqual(("error: hrp must not be longer than 83 characters\n", 1),
                Run("encode", "--hrp", new string('a', hrpLength), "12ef"));
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
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithHrpContainingInvalidCharacters(string hrp)
        {
            Assert.AreEqual(("error: hrp contains invalid characters\n", 1),
                Run("encode", "--hrp", hrp, "12ef"));
        }

        [Test]
        public void HrpValidationTakesPrecedenceBeforeDataValidation()
        {
            Assert.AreEqual(("error: hrp contains invalid characters\n", 1), Run("encode", "--hrp", "\x14", "xy"));
        }

        /** *************** **/
        /** Data validation **/
        /** *************** **/
        // TODO add the same test again but with input from file (or parameterize)
        [Test]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndWithNonHexData()
        {
            Assert.AreEqual(("error: data are not in hex format\n", 1), Run("encode", "--hrp", "abc", "xy"));
        }

        [TestCase("xy")]
        [TestCase("Eu8=")]
        [TestCase("0001001011101111")]
        public void PrintsErrorMessageAndExitsWithOneWheCalledWithValidHrpAndFormatHexAndWithNonHexData(
            string data)
        {
            Assert.AreEqual(("error: data are not in hex format\n", 1),
                Run("encode", "--hrp", "abc", "--format", "hex", data));
        }

        [TestCase("xy")]
        [TestCase("12ef")]
        [TestCase("0001001011101111")]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndFormatBase64AndWithNonBase64Data(
            string data)
        {
            Assert.AreEqual(("error: data are not in base64 format\n", 1),
                Run("encode", "--hrp", "abc", "--format", "base64", data));
        }

        [TestCase("xy")]
        [TestCase("12ef")]
        [TestCase("Eu8=")]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndFormatBinaryAndWithNonBinaryData(
            string data)
        {
            Assert.AreEqual(("error: data are not in binary format\n", 1),
                Run("encode", "--hrp", "abc", "--format", "binary", data));
        }

        // TODO output is not hex but ascii
        [TestCase("hex", "12ef", "abc1zths84d6rg")]
        [TestCase("base64", "Eu8=", "YWJjMXp0aHM4NGQ2cmc=")]
        [TestCase("binary", "0001001011101111",
            "0110000101100010011000110011000101111010011101000110100001110011001110000011010001100100001101100111001001100111")]
        public void PrintsEncodedStringAndExitsWithZeroWhenEncodeCalledWithValidHrpAndFormatAndDataInGivenFormat(
            string format, string data, string expected)
        {
            Assert.AreEqual(($"{expected}\n", 0), Run("encode", "--hrp", "abc", "--format", format, data));
        }

        /** ****** **/
        /** Output **/
        /** ****** **/
        [Test]
        public void
            SavesEncodedStringInHexAndExitsWithZeroWhenCalledWithValidHrpAndValidDataAndOutputFileAndWithoutFormatArgument()
        {
            Assert.AreEqual(("", 0), Run("encode", "--hrp", "abc", "--output", TestOutputFile, "12ef"));
            Assert.AreEqual("abc1zths84d6rg", File.ReadAllText(TestOutputFile));
        }

        [Test]
        public void
            SavesEncodedStringInHexAndExitsWithZeroWhenCalledWithValidHrpAndValidDataAndOutputFileAndFormatHexArgument()
        {
            Assert.AreEqual(("", 0),
                Run("encode", "--hrp", "abc", "--output", TestOutputFile, "12ef", "--format", "hex"));
            Assert.AreEqual("abc1zths84d6rg", File.ReadAllText(TestOutputFile));
        }

        /** ************** **/
        /** Standard input **/
        /** ************** **/
        [Test]
        public void ReadsDataFromStdinInHexWhenNeitherInputNorDataAreGiven()
        {
            // TODO somehow write into stdin
            Assert.AreEqual(("abc1zths84d6rg\n", 0), Run("encode", "--hrp", "abc"));
        }

        [TestCase("hex", "12ef", "abc1zths84d6rg")]
        [TestCase("base64", "Eu8=", "YWJjMXp0aHM4NGQ2cmc=")]
        [TestCase("binary", "0001001011101111",
            "0110000101100010011000110011000101111010011101000110100001110011001110000011010001100100001101100111001001100111")]
        public void ReadsDataFromStdinInGivenFormatWhenNeitherInputNorDataAreGiven(string format, string data,
            string expected)
        {
            // TODO somehow write into stdin
            Assert.AreEqual(($"{expected}\n", 0), Run("encode", "--hrp", "abc", "--format", format));
        }

        [Test]
         public void ReadsBinaryDataFromStdinWhenNeitherInputNorDataAreGivenAndBinaryFormatIsSpecified()
        {
            var data = "0001001011101111";
            // TODO somehow write into stdin
            Assert.AreEqual(
                ($"0110000101100010011000110011000101111010011101000110100001110011001110000011010001100100001101100111001001100111\n",
                    0), Run("encode", "--hrp", "abc", "--format", "binary"));
        }
    }
}
