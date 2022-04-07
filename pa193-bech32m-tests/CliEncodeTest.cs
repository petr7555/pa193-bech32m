using System;
using System.IO;
using System.Linq;
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
  -f, --format <format>      format of input data (choices: ""hex"", ""base64"", ""binary"", default: ""hex"")
  -i, --input <inputfile>    input file with data. For ""hex"" and ""base64"" format the newline characters are ignored.
  -o, --output <outputfile>  output file where result will be saved. If not present, result is printed to stdout.
  -h, --help                 display help for command
";

        private const string TestInputHexFile = "test_input_hex_file";
        private const string TestInputBinaryFile = "test_input_binary_file";
        private const string TestInputBase64File = "test_input_base64_file";
        private const string TestInputRandomCharactersFile = "test_input_random_characters_file";
        private const string TestInputEmptyFile = "test_input_empty_file";
        private const string TestInputFile = "test_input_file";

        private const string TestOutputFile = "test_output_file";

        [OneTimeSetUp]
        public void BeforeAll()
        {
            File.WriteAllText(TestInputHexFile, "12ef34");
            File.WriteAllBytes(TestInputBinaryFile, new byte[] {18, 239, 52});
            File.WriteAllText(TestInputBase64File, "Eu80");
            File.WriteAllText(TestInputRandomCharactersFile, "xy");
            File.WriteAllText(TestInputEmptyFile, "");
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
            File.Delete(TestInputHexFile);
            File.Delete(TestInputBinaryFile);
            File.Delete(TestInputBase64File);
            File.Delete(TestInputRandomCharactersFile);
            File.Delete(TestInputEmptyFile);
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
            Assert.AreEqual(("Result:\n--format1zthngmmh5e5\n", 0), Run("encode", "--hrp", "--format", "12ef34"));
            Assert.AreEqual(("Result:\n-f1zthngfqlyj9\n", 0), Run("encode", "--hrp", "-f", "12ef34"));
        }

        [Test]
        public void IgnoresOtherArgumentsWhenAllNecessaryParametersAreProvided()
        {
            Assert.AreEqual(("Result:\nabc1zthng4l66t2\n", 0), Run("encode", "--hrp", "abc", "12ef34", "xy", "ab"));
        }

        [Test]
        public void
            PrintsEncodedStringAndExitsWithZeroWhenCalledWithValidHrpAndWithHexDataBeforeOrAfterHrpOption()
        {
            Assert.AreEqual(("Result:\nabc1zthng4l66t2\n", 0), Run("encode", "12ef34", "--hrp", "abc"));
            Assert.AreEqual(("Result:\nabc1zthng4l66t2\n", 0), Run("encode", "--hrp", "abc", "12ef34"));
        }

        [Test]
        public void PrintsEncodedStringAndExitsWithZeroWhenCalledWithValidHrpAndDataAreBetweenOptions()
        {
            Assert.AreEqual(("Result:\nabc1zthng4l66t2\n", 0),
                Run("encode", "--format", "hex", "12ef34", "--hrp", "abc"));
        }

        [Test]
        public void PrintsUnknownOptionAndExitsWithOneWhenCalledWithUnknownOption()
        {
            Assert.AreEqual(("error: unknown option '--bar'\n", 1), Run("encode", "--hrp", "abc", "12ef34", "--bar"));
            Assert.AreEqual(("error: unknown option '-b'\n", 1), Run("encode", "--hrp", "abc", "12ef34", "-b"));
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
            Assert.AreEqual(("error: hrp must not be empty\n", 1), Run("encode", "--hrp", "", "12ef34"));
        }

        [TestCase(84)]
        [TestCase(1_000_000)]
        [TestCase(1_000_001)]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithTooLongHrp(int hrpLength)
        {
            Assert.AreEqual(("error: hrp must not be longer than 83 characters\n", 1),
                Run("encode", "--hrp", new string('a', hrpLength), "12ef34"));
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
                Run("encode", "--hrp", hrp, "12ef34"));
        }

        [Test]
        public void HrpValidationTakesPrecedenceBeforeDataValidation()
        {
            Assert.AreEqual(("error: hrp contains invalid characters\n", 1), Run("encode", "--hrp", "\x14", "xy"));
        }

        /** *********************** **/
        /** Data passed as argument **/
        /** *********************** **/
        [TestCase("hex", "12ef34")]
        [TestCase("base64", "Eu80")]
        public void PrintsEncodedStringAndExitsWithZeroWhenEncodeCalledWithValidHrpAndFormatAndDataInGivenFormat(
            string format, string data)
        {
            Assert.AreEqual(("Result:\nabc1zthng4l66t2\n", 0), Run("encode", "--hrp", "abc", "--format", format, data));
        }

        [TestCase("xy")]
        [TestCase("12ef34")]
        [TestCase("Eu80")]
        [TestCase("0001001011101111")]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndFormatBinaryAndAnyData(
            string data)
        {
            Assert.AreEqual(("error: binary data cannot be passed as command-line argument\n", 1),
                Run("encode", "--hrp", "abc", "--format", "binary", data));
        }

        [Test]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndWithNonHexData()
        {
            Assert.AreEqual(("error: data are not in hex format\n", 1), Run("encode", "--hrp", "abc", "xy"));
        }

        [TestCase("xy")]
        [TestCase("Eu80")]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndFormatHexAndWithNonHexData(
            string data)
        {
            Assert.AreEqual(("error: data are not in hex format\n", 1),
                Run("encode", "--hrp", "abc", "--format", "hex", data));
        }

        [TestCase("xy")]
        [TestCase("12ef34")]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndFormatBase64AndWithNonBase64Data(
            string data)
        {
            Assert.AreEqual(("error: data are not in base64 format\n", 1),
                Run("encode", "--hrp", "abc", "--format", "base64", data));
        }

        [TestCase("hex")]
        [TestCase("base64")]
        public void CanReadEmptyDataArgumentInAnyFormat(string format)
        {
            Assert.AreEqual(($"Result:\nabc1k7c8sc\n", 0),
                RunWithInput("", "encode", "--hrp", "abc", "--format", format, ""));
        }

        /** ************************* **/
        /** Data passed through stdin **/
        /** ************************* **/
        [Test]
        public void ReadsDataFromStdinInHexWhenNeitherInputNorDataAreGiven()
        {
            Assert.AreEqual(("Enter data in hex format. Press Enter when done.\n\nResult:\nabc1zthng4l66t2\n", 0),
                RunWithInput("12ef34", "encode", "--hrp", "abc"));
        }

        [TestCase("hex", "12ef34")]
        [TestCase("base64", "Eu80")]
        public void ReadsDataFromStdinInGivenFormatWhenNeitherInputNorDataAreGiven(string format, string data)
        {
            Assert.AreEqual(($"Enter data in {format} format. Press Enter when done.\n\nResult:\nabc1zthng4l66t2\n", 0),
                RunWithInput(data, "encode", "--hrp", "abc", "--format", format));
        }

        [Test]
        public void ReadsBinaryDataFromStdinWhenNeitherInputNorDataAreGivenAndBinaryFormatIsSpecified()
        {
            Assert.AreEqual(("Enter data in binary format. Press Enter when done.\n\nResult:\nabc1zthng4l66t2\n", 0),
                RunWithBinaryInput(new byte[] {18, 239, 52}, "encode", "--hrp", "abc", "--format", "binary"));
        }

        [TestCase("xy")]
        [TestCase("Eu80")]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndFormatHexAndWithNonHexStdinData(
            string data)
        {
            Assert.AreEqual(
                ("Enter data in hex format. Press Enter when done.\n\nerror: data are not in hex format\n", 1),
                RunWithInput(data, "encode", "--hrp", "abc", "--format", "hex"));
        }

        [Test]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndFormatHexAndWithBinaryStdinData()
        {
            Assert.AreEqual(
                ("Enter data in hex format. Press Enter when done.\n\nerror: data are not in hex format\n", 1),
                RunWithBinaryInput(new byte[] {18, 230}, "encode", "--hrp", "abc", "--format", "hex"));
        }

        [TestCase("xy")]
        [TestCase("12ef34")]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndFormatBase64AndWithNonBase64StdinData(
            string data)
        {
            Assert.AreEqual(("error: data are not in base64 format\n", 1),
                RunWithInput(data, "encode", "--hrp", "abc", "--format", "base64", data));
        }

        [Test]
        public void PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndFormatBase64AndWithBinaryStdinData()
        {
            Assert.AreEqual(
                ("Enter data in base64 format. Press Enter when done.\n\nerror: data are not in base64 format\n", 1),
                RunWithBinaryInput(new byte[] {18, 230}, "encode", "--hrp", "abc", "--format", "base64"));
        }

        [TestCase("hex")]
        [TestCase("base64")]
        public void CanReadEmptyStdinInAnyFormat(string format)
        {
            Assert.AreEqual(($"Enter data in {format} format. Press Enter when done.\n\nResult:\nabc1k7c8sc\n", 0),
                RunWithInput("", "encode", "--hrp", "abc", "--format", format));
        }

        [TestCase("hex")]
        [TestCase("base64")]
        [TestCase("binary")]
        public void HandlesWhenReadLineReturnsNull(string format)
        {
            Assert.AreEqual(($"Enter data in {format} format. Press Enter when done.\n\nResult:\nabc1k7c8sc\n", 0),
                Run("encode", "--hrp", "abc", "--format", format));
        }

        [Test]
        public void CanReadEmptyStdinInBinaryFormat()
        {
            Assert.AreEqual(("Enter data in binary format. Press Enter when done.\n\nResult:\nabc1k7c8sc\n", 0),
                RunWithBinaryInput(Array.Empty<byte>(), "encode", "--hrp", "abc", "--format", "binary"));
        }

        /** ****************************** **/
        /** Data passed through input file **/
        /** ****************************** **/
        [Test]
        public void PrintsEncodedStringAndExitsWithZeroWhenCalledWithValidHrpAndInputFileAndWithoutData()
        {
            Assert.AreEqual(("Result:\nabc1zthng4l66t2\n", 0),
                Run("encode", "--hrp", "abc", "--input", TestInputHexFile));
        }

        [TestCase("12ef34")]
        [TestCase("xy")]
        public void InputParameterTakesPrecedenceBeforePassedDataWhichCanEvenBeInvalid(string data)
        {
            Assert.AreEqual(("Result:\nabc1zthng4l66t2\n", 0),
                Run("encode", "--hrp", "abc", data, "--input", TestInputHexFile));
        }

        [TestCase("hex", TestInputHexFile)]
        [TestCase("base64", TestInputBase64File)]
        [TestCase("binary", TestInputBinaryFile)]
        public void ReadsDataFromInputFileInGivenFormatWhenInputFileIsGiven(string format, string inputFileName)
        {
            Assert.AreEqual(("Result:\nabc1zthng4l66t2\n", 0),
                Run("encode", "--hrp", "abc", "--format", format, "--input", inputFileName));
        }

        [TestCase(TestInputBase64File)]
        [TestCase(TestInputBinaryFile)]
        [TestCase(TestInputRandomCharactersFile)]
        public void
            PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndFormatHexAndWithInputFileContainingNonHexData(
                string inputFileName)
        {
            Assert.AreEqual(("error: data are not in hex format\n", 1),
                Run("encode", "--hrp", "abc", "--format", "hex", "--input", inputFileName));
        }

        [TestCase(TestInputHexFile)]
        [TestCase(TestInputBinaryFile)]
        [TestCase(TestInputRandomCharactersFile)]
        public void
            PrintsErrorMessageAndExitsWithOneWhenCalledWithValidHrpAndFormatBase64AndWithInputFileContainingNonBase64Data(
                string inputFileName)
        {
            Assert.AreEqual(("error: data are not in base64 format\n", 1),
                Run("encode", "--hrp", "abc", "--format", "base64", "--input", inputFileName));
        }

        [TestCase(TestInputHexFile, "abc1xyex2e3nxsjgjava")]
        [TestCase(TestInputBase64File, "abc1g46nsvqyql3j3")]
        [TestCase(TestInputRandomCharactersFile, "abc10pusrll0u7")]
        public void CanReadFileWithAnyContentAsBinaryData(string inputFileName, string expected)
        {
            Assert.AreEqual(($"Result:\n{expected}\n", 0),
                Run("encode", "--hrp", "abc", "--format", "binary", "--input", inputFileName));
        }

        [TestCase("hex")]
        [TestCase("base64")]
        [TestCase("binary")]
        public void CanReadEmptyFileInAnyFormat(string format)
        {
            Assert.AreEqual(($"Result:\nabc1k7c8sc\n", 0),
                Run("encode", "--hrp", "abc", "--format", format, "--input", TestInputEmptyFile));
        }

        [TestCase("hex")]
        [TestCase("base64")]
        [TestCase("binary")]
        public void PrintsErrorMessageAndExitsWithOneWhenInputFileDoesNotExistForAnyFormat(string format)
        {
            const string nonexistentInputFileName = "nonexistent_file";
            Assert.AreEqual(($"error: input file {nonexistentInputFileName} does not exist\n", 1),
                Run("encode", "--hrp", "abc", "--format", format, "--input", nonexistentInputFileName));
        }

        /** ****** **/
        /** Output **/
        /** ****** **/
        [Test]
        public void
            SavesEncodedStringToOutputFileAndExitsWithZeroWhenCalledWithValidHrpAndValidDataAndOutputFileAndWithoutFormatArgument()
        {
            Assert.AreEqual(("", 0), Run("encode", "--hrp", "abc", "--output", TestOutputFile, "12ef34"));
            Assert.AreEqual("abc1zthng4l66t2", File.ReadAllText(TestOutputFile));
        }

        [TestCase("hex", "12ef34")]
        [TestCase("base64", "Eu80")]
        public void
            SavesEncodedStringToOutputFileAndExitsWithZeroWhenCalledWithValidHrpAndFormatAndDataInGivenFormat(
                string format, string data)
        {
            Assert.AreEqual(("", 0),
                Run("encode", "--hrp", "abc", "--output", TestOutputFile, data, "--format", format));
            Assert.AreEqual("abc1zthng4l66t2", File.ReadAllText(TestOutputFile));
        }

        [Test]
        public void
            SavesEncodedStringToOutputFileAndExitsWithZeroWhenCalledWithValidHrpAndFormatBinaryAndBinaryStdinData()
        {
            Assert.AreEqual(("Enter data in binary format. Press Enter when done.\n\n", 0),
                RunWithBinaryInput(new byte[] {18, 239, 52}, "encode", "--hrp", "abc", "--output", TestOutputFile,
                    "--format", "binary"));
            Assert.AreEqual("abc1zthng4l66t2", File.ReadAllText(TestOutputFile));
        }

        [TestCase("hex", 1024)]
        [TestCase("hex", 4096)]
        [TestCase("hex", 1_000_000)]
        [TestCase("base64", 1024)]
        [TestCase("base64", 4096)]
        [TestCase("base64", 1_000_000)]
        public void HandlesLargeHexAndBase64DataFromPipe(string format, int length)
        {
            var (output, code) = RunWithInput(new string('a', length), "encode", "--hrp", "abc", "--format", format);

            StringAssert.DoesNotContain("error", output);
            Assert.AreEqual(0, code);
        }

        [TestCase(1024)]
        [TestCase(4096)]
        [TestCase(1_000_000)]
        [TestCase(1_000_001)]
        public void HandlesLargeBinaryDataFromPipe(int length)
        {
            var (output, code) = RunWithBinaryInput(Enumerable.Repeat((byte) 94, length).ToArray(), "encode", "--hrp",
                "abc", "--format", "binary");

            StringAssert.DoesNotContain("error", output);
            Assert.AreEqual(0, code);
        }

        [TestCase("hex", 1024)]
        [TestCase("hex", 4096)]
        [TestCase("hex", 1_000_000)]
        [TestCase("base64", 1024)]
        [TestCase("base64", 4096)]
        [TestCase("base64", 1_000_000)]
        public void HandlesLargeHexAndBase64DataFromFile(string format, int length)
        {
            File.WriteAllText(TestInputFile, new string('a', length));

            var (output, code) = Run("encode", "--hrp", "abc", "--format", format, "--input", TestInputFile);

            StringAssert.DoesNotContain("error", output);
            Assert.AreEqual(0, code);
        }

        [TestCase(1024)]
        [TestCase(4096)]
        [TestCase(1_000_000)]
        [TestCase(1_000_001)]
        public void HandlesLargeBinaryDataFromFile(int length)
        {
            File.WriteAllBytes(TestInputFile, Enumerable.Repeat((byte) 94, length).ToArray());

            var (output, code) = Run("encode", "--hrp", "abc", "--format", "binary", "--input", TestInputFile);

            StringAssert.DoesNotContain("error", output);
            Assert.AreEqual(0, code);
        }

        [TestCase("hex", "12ef34\n")]
        [TestCase("hex", "12e\nf\n34\n")]
        [TestCase("hex", "12ef34\r\n")]
        [TestCase("hex", "12e\r\nf\r\n34\r\n")]
        [TestCase("hex", "12e\nf\r\n34\n")]
        [TestCase("base64", "Eu80\n")]
        [TestCase("base64", "E\nu\n80\n")]
        [TestCase("base64", "Eu80\r\n")]
        [TestCase("base64", "E\r\nu\r\n80\r\n")]
        [TestCase("base64", "E\nu\r\n80\n")]
        public void IgnoresNewlinesInHexAndBase64File(string format, string contents)
        {
            File.WriteAllText(TestInputFile, contents);
            Assert.AreEqual(("Result:\nabc1zthng4l66t2\n", 0),
                Run("encode", "--hrp", "abc", "--format", format, "--input", TestInputFile));
        }

        // \r = CR = \x0d = 13
        // \n = LF = \x0a = 10
        // 12 ef 34 0d 0a
        [TestCase(new byte[] {18, 239, 52, 13, 10}, "abc1zthngrg2hys5s3")]
        [TestCase(new byte[] {18, 239, 52, 10}, "abc1zthngzs6takgr")]
        [TestCase(new byte[] {18, 10, 239, 52, 10}, "abc1zg9w7dq2g2j5ha")]
        [TestCase(new byte[] {18, 13, 10, 239, 52, 13, 10}, "abc1zgxs4me5p59qw80x8e")]
        [TestCase(new byte[] {18, 13, 10, 239, 52, 10}, "abc1zgxs4me5pgk50wlg")]
        public void DoesNoIgnoreNewlinesInBinaryFile(byte[] contents, string expected)
        {
            File.WriteAllBytes(TestInputFile, contents);
            Assert.AreEqual(($"Result:\n{expected}\n", 0),
                Run("encode", "--hrp", "abc", "--format", "binary", "--input", TestInputFile));
        }
    }
}
