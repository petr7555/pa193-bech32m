using System.IO;
using System.Text;
using NUnit.Framework;
using pa193_bech32m.CLI;

namespace pa193_bech32m_tests
{
    public class CliTest
    {
        private const string Usage = @"Usage: bech32m [options] [command]

Bech32m encoder and decoder

Options:
  -V, --version            output the version number
  -h, --help               display help for command

Commands:
  encode [options] <data>  encode hrp and data into Bech32m string
  help [command]           display help for command
";

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
            Assert.AreEqual((Usage, 0), Run(helpFlag));
        }

        [Test]
        public void PrintsUsageAndExitsWithZeroOnHelpCommandWithoutArguments()
        {
            Assert.AreEqual((Usage, 0), Run("help"));
        }

        [Test]
        public void PrintsUsageAndExitsWithOneWhenNothingPassed()
        {
            Assert.AreEqual((Usage, 1), Run());
        }
    }
}
