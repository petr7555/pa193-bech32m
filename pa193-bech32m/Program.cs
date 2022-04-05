using pa193_bech32m.CLI;

namespace pa193_bech32m
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var cli = new Cli();
            return cli.Run(args);
        }
    }
}
