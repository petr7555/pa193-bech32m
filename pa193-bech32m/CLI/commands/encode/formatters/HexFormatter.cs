using pa193_bech32m.CLI.commands.encode.readers;

namespace pa193_bech32m.CLI.commands.encode.formatters
{
    internal class HexFormatter : Formatter
    {
        public HexFormatter(IReader reader) : base(reader)
        {
        }

        public override (bool hasError, string data) GetHexData()
        {
            var (hasError, data) = Reader.ReadString();
            if (hasError)
            {
                return (true, "");
            }

            if (!Bech32m.IsValidHexInput(data))
            {
                Cli.PrintError("data are not in hex format");
                return (true, "");
            }

            return (false, data);
        }
    }
}
