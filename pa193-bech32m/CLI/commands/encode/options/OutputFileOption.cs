namespace pa193_bech32m.CLI.commands.encode.options
{
    public class OutputFileOption : IOption
    {
        public string Description() => "output file where result will be saved";
        public string Flags() => "-o, --out <outputfile>";
        public bool IsValidOption(string arg) => arg == "-o" || arg == "--out";

        public int Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
