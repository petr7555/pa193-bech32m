using System.IO;
using System.Text;
using NUnit.Framework;
using pa193_bech32m.CLI;

namespace pa193_bech32m_tests
{
    public class CliTest
    {
        private static string Run(params string[] args)
        {
            var outMemoryStream = new MemoryStream();

            using (var outStreamWriter = new StreamWriter(outMemoryStream))
            {
                var cli = new Cli(outStreamWriter);
                cli.Run(args);
            }

            return Encoding.Default.GetString(outMemoryStream.ToArray());
        }

        [Test]
        public void PrintsVersion()
        {
            Assert.AreEqual("0.0.1\n", Run("-V"));
            Assert.AreEqual("0.0.1\n", Run("--version"));
        }
    }
}
