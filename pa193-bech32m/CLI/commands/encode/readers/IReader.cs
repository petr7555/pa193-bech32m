namespace pa193_bech32m.CLI.commands.encode.readers
{
    internal interface IReader
    {
        public (bool hasError, string data) ReadString();
        public (bool hasError, byte[] data) ReadBytes();
    }
}
