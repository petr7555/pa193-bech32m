namespace pa193_bech32m.CLI.commands.encode.arguments
{
    public class DataArgument : IArgument
    {
        public string Flags() => "data";

        public string Description() =>
            "data to encode. If neither this argument nor \"--input\" is given, data are read from stdin.";
    }
}
