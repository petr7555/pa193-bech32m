namespace pa193_bech32m.CLI.commands.encode.options
{
    public class InputFileOption : IOption
    {
        public string Description() => "input file with data";
        public string Flags() => "-i, --input <inputfile>";
        public bool IsValidOption(string arg) => arg == "-i" || arg == "--input";

        public int Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
