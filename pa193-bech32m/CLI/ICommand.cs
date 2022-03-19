namespace pa193_bech32m.CLI
{
    public interface ICommand
    {
        public string Name();
        public string Description();
        public int Execute(string[] args);
    }
}
