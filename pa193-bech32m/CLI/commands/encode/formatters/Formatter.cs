using pa193_bech32m.CLI.commands.encode.readers;

namespace pa193_bech32m.CLI.commands.encode.formatters
{
    internal abstract class Formatter
    {
        protected readonly IReader Reader;

        protected Formatter(IReader reader)
        {
            Reader = reader;
        }

        public abstract (bool hasError, string data) GetHexData();
    }
}
