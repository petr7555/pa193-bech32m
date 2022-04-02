namespace pa193_bech32m.CLI.commands.encode.arguments
{
    public class DataArgument : IArgument
    {
        public string Flags() => "data";
        public string Description() => "data to encode, required if \"--input\" is not specified";
    }
}
