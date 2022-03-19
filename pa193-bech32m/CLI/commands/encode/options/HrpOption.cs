namespace pa193_bech32m.CLI.commands.encode.options
{
    public class HrpOption : IOption
    {
        public string Flags() => "--hrp <hrp>";
        public string Description() => "human-readable part";
        public bool IsValidOption(string arg) => arg == "--hrp";
        
        public int Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
