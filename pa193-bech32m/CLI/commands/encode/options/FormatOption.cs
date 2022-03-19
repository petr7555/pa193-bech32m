namespace pa193_bech32m.CLI.commands.encode.options
{
    public class FormatOption : IOption
    {
        public string Description() => "format of input data (choices: \"hex\", \"base64\", \"binary\", default: \"hex\")";
        public string Flags() => "-f, --format <format>";
        public bool IsValidOption(string arg) => arg == "-f" || arg == "--format";

        public int Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
