namespace pa193_bech32m.CLI.commands.encode.options
{
    public class HrpOption : IOption
    {
        public string Description() => "human-readable part";
        public string Flags() => "--hrp <hrp>";
        public bool IsValidOption(string arg) => arg == "--hrp";
        public int Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
