using System;
using pa193_bech32m.CLI.commands.encode.readers;

namespace pa193_bech32m.CLI.commands.encode.formatters
{
    internal class BinaryFormatter : Formatter
    {
        public BinaryFormatter(IReader reader) : base(reader)
        {
        }

        public override (bool hasError, string data) GetHexData()
        {
            var (hasError, data) = Reader.ReadBytes();
            if (hasError)
            {
                return (true, "");
            }

            return (false, Convert.ToHexString(data));
        }
    }
}
