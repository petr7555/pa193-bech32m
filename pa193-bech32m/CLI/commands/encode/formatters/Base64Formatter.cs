using System;
using pa193_bech32m.CLI.commands.encode.readers;

namespace pa193_bech32m.CLI.commands.encode.formatters
{
    internal class Base64Formatter : Formatter
    {
        public Base64Formatter(IReader reader) : base(reader)
        {
        }

        public override (bool hasError, string data) GetHexData()
        {
            var (hasError, data) = Reader.ReadString();
            if (hasError)
            {
                return (true, "");
            }

            try
            {
                return (false, Convert.ToHexString(Convert.FromBase64String(data)));
            }
            catch (FormatException)
            {
                Cli.PrintError("data are not in base64 format");
                return (true, "");
            }
        }
    }
}
