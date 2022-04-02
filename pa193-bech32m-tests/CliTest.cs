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

        public static (string, int) Run(params string[] args)
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
    }
}
