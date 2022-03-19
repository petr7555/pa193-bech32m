using System;

namespace pa193_bech32m.CLI.options
{
    public class HelpOption : IOption
    {
        private readonly Action _usage;

        public HelpOption(Action usage)
        {
            _usage = usage;
        }

        public string Description() => "display help for command";
        public string Flags() => "-h, --help";
        public bool IsValidOption(string arg) => arg == "-h" || arg == "--help";

        public int Execute()
        {
            _usage();
            return Cli.ExitSuccess;
        }
    }
}
