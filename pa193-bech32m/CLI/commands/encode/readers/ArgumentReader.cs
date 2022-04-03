using System;

namespace pa193_bech32m.CLI.commands.encode.readers
{
    internal class ArgumentReader : IReader
    {
        private readonly string _argument;

        public ArgumentReader(string argument)
        {
            _argument = argument;
        }

        public (bool hasError, string data) ReadString()
        {
            return (false, _argument);
        }

        public (bool hasError, byte[] data) ReadBytes()
        {
            Cli.PrintError("binary data cannot be passed as command-line argument");
            return (true, Array.Empty<byte>());
        }
    }
}
