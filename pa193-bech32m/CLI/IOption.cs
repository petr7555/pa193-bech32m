namespace pa193_bech32m.CLI
{
    public interface IOption
    {
        public string Description();
        public string Flags();
        public bool IsValidOption(string arg);
        public int Execute();
    }
}
