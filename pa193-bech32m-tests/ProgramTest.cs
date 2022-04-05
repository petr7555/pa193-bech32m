using NUnit.Framework;
using pa193_bech32m;

namespace pa193_bech32m_tests
{
    public class ProgramTest
    {
        [Test]
        public void CanRunProgram()
        {
            Assert.AreEqual(0, Program.Main(new[] {"--help"}));
        }
    }
}
