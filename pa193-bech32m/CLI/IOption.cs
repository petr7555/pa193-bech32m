namespace pa193_bech32m.CLI
{
    public interface IOption
    {
        public string Flags();
        public string Description();
        public bool IsValidOption(string arg);
        public int Execute();
    }
}
