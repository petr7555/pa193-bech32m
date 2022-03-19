namespace pa193_bech32m.CLI
{
    public interface ICommand
    {
        public string Name();
        public string Description();
        public void Execute(string[] args);
    }
}
