using System.IO;
using System.Text;
using NUnit.Framework;
using pa193_bech32m.CLI;

namespace pa193_bech32m_tests
{
    public class CliTest
    {
        private const string CliUsage = @"Usage: bech32m [options] [command]

Bech32m encoder and decoder

Options:
  -V, --version            output the version number
  -h, --help               display help for command

Commands:
  encode [options] <data>  encode hrp and data into Bech32m string
  help [command]           display help for command
";

        private const string EncodeUsage = @"Usage: bech32m encode [options] <data>

encode hrp and data into Bech32m string

Arguments:
  data                       data to encode, required if ""--input"" is not specified

Options:
  --hrp <hrp>                human-readable part
  -f, --format <format>      format of input data (choices: ""hex"", ""base64"", ""binary"", default: ""hex"")
  -i, --input <inputfile>    input file with data
  -o, --output <outputfile>  output file where result will be saved. If not present, result is printed to stdout.
  -h, --help                 display help for command
";

        private const string TestInputFile = "test_input_file";
        private const string TestOutputFile = "test_output_file";

        private static (string, int) Run(params string[] args)
        {
            var outMemoryStream = new MemoryStream();
            int exitCode;
            using (var outStreamWriter = new StreamWriter(outMemoryStream))
            {
                var cli = new Cli(outStreamWriter);
                exitCode = cli.Run(args);
            }

            return (Encoding.Default.GetString(outMemoryStream.ToArray()), exitCode);
        }

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

        [TestCase("-V")]
        [TestCase("--version")]
        public void PrintsVersionAndExistsWithZeroOnVersionFlag(string versionFlag)
        {
            Assert.AreEqual(("0.0.1\n", 0), Run(versionFlag));
        }

        [TestCase("-h")]
        [TestCase("--help")]
        public void PrintsUsageAndExitsWithZeroOnHelpFlag(string helpFlag)
        {
            Assert.AreEqual((CliUsage, 0), Run(helpFlag));
        }

        [TestCase("-h")]
        [TestCase("--help")]
        public void PrintsUsageAndExitsWithZeroWhenHelpFlagPassedAnywhereButNotAfterValidCommand(string helpFlag)
        {
            Assert.AreEqual((CliUsage, 0), Run("foo", "--bar", helpFlag, "baz"));
        }

        [Test]
        public void PrintsUsageAndExitsWithZeroOnHelpCommandWithoutArguments()
        {
            Assert.AreEqual((CliUsage, 0), Run("help"));
        }

        [Test]
        public void PrintsUsageAndExitsWithOneOnHelpCommandWithUnknownArgument()
        {
            Assert.AreEqual((CliUsage, 1), Run("help", "foo"));
        }

        [Test]
        public void PrintsUsageAndExitsWithOneWhenNothingPassed()
        {
            Assert.AreEqual((CliUsage, 1), Run());
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
        public void PrintsUnknownCommandAndExitsWithOneWhenPassedUnknownCommand()
        {
            Assert.AreEqual(("error: unknown command 'foo'\n", 1), Run("foo"));
        }

        [Test]
        public void PrintsUnknownOptionAndExitsWithOneWhenPassedUnknownOption()
        {
            Assert.AreEqual(("error: unknown option '--foo'\n", 1), Run("--foo"));
            Assert.AreEqual(("error: unknown option '-f'\n", 1), Run("-f"));
        }

        [Test]
        public void PrintsRequiredOptionNotSpecifiedAndExitsWithOneWhenEncodeCalledWithoutHrp()
        {
            Assert.AreEqual(("error: required option '--hrp <hrp>' not specified\n", 1), Run("encode"));
        }

        [Test]
        public void PrintsOptionArgumentMissingAndExitsWithOneWhenEncodeCalledWithHrpOptionWithoutHrpArgument()
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
            PrintsMissingRequiredArgumentAndExitsWithOneWhenEncodeCalledWithHrpOptionAndHrpArgumentButWithoutDataAndWithoutInputFile()
        {
            Assert.AreEqual(("error: missing required argument 'data'\n", 1), Run("encode", "--hrp", "abc"));
        }

        [Test]
        public void
            PrintsEncodedStringAndExitsWithZeroWhenEncodeCalledWithValidHrpAndInputFileAndWithoutData()
        {
            Assert.AreEqual(("abc1zths84d6rg\n", 0), Run("encode", "--hrp", "abc", "--input", TestInputFile));
        }

        [Test]
        public void
            PrintsEncodedStringAndExitsWithZeroWhenEncodeCalledWithHrpOptionAndHrpArgumentAndWithHexDataBeforeOrAfterHrpOption()
        {
            Assert.AreEqual(("abc1zths84d6rg\n", 0), Run("encode", "12ef", "--hrp", "abc"));
            Assert.AreEqual(("abc1zths84d6rg\n", 0), Run("encode", "--hrp", "abc", "12ef"));
        }

        [Test]
        public void PrintsEncodedStringAndExitsWithZeroWhenHrpIsValidAndDataAreBetweenOptions()
        {
            Assert.AreEqual(("abc1zths84d6rg\n", 0), Run("encode", "--format", "hex", "12ef", "--hrp", "abc"));
        }

        [Test]
        public void PrintsEncodedStringAndExitsWithZeroWhenEncodeCalledWithHrpOptionAndHrpArgumentAndWithHexData()
        {
            Assert.AreEqual(("abc1zths84d6rg\n", 0), Run("encode", "--hrp", "abc", "12ef"));
        }

        [Test]
        public void PrintsInvalidInputAndExitsWithOneWhenEncodeCalledWithHrpOptionAndHrpArgumentAndWithNonHexData()
        {
            Assert.AreEqual(("error: invalid input\n", 1), Run("encode", "--hrp", "abc", "xy"));
        }

        [Test]
        public void PrintsUnknownOptionAndExitsWithOneWhenEncodeCalledWithUnknownOption()
        {
            Assert.AreEqual(("error: unknown option '--bar'\n", 1), Run("encode", "--hrp", "abc", "12ef", "--bar"));
            Assert.AreEqual(("error: unknown option '-b'\n", 1), Run("encode", "--hrp", "abc", "12ef", "-b"));
        }

        [Test]
        public void IgnoresOtherArgumentsWhenAllNecessaryParametersForEncodeAreProvided()
        {
            Assert.AreEqual(("abc1zths84d6rg\n", 0), Run("encode", "--hrp", "abc", "12ef", "xy", "ab"));
        }

        [Test]
        public void
            IgnoresOtherArgumentsWhenAllNecessaryParametersForEncodeAreProvidedInputParamTakesPrecedenceBeforePassedData()
        {
            Assert.AreEqual(("abc1zths84d6rg\n", 0),
                Run("encode", "--hrp", "abc", "xy", "--input", TestInputFile, "ab"));
        }

        [Test]
        public void PrintsInvalidArgumentAndExitsWithOneWhenEncodeCalledWithFormatOptionAndInvalidFormatArgument()
        {
            Assert.AreEqual(
                ("error: option '-f, --format <format>' argument 'base10' is invalid. Allowed choices are hex, base64, binary.\n",
                    1), Run("encode", "--format", "base10"));
        }
        
        [Test]
        public void SavesEncodedStringAndExitsWithZeroWhenEncodeCalledWithValidHrpAndOutputFile()
        {
            Assert.AreEqual(("", 0), Run("encode", "--hrp", "abc", "--output", TestOutputFile, "12ef"));
            Assert.AreEqual("abc1zths84d6rg", File.ReadAllText(TestOutputFile));
        }
    }
}
