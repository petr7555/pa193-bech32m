namespace pa193_bech32m.CLI.commands.encode.options
{
    public class HrpOption : IOption
    {
        public string Description() => "human-readable part";
        public string Flags() => "-h, --hrp <hrp>";
        public bool IsValidOption(string arg) => arg == "-h" || arg == "--hrp";
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
