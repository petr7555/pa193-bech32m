using System;

namespace pa193_bech32m.CLI.commands.help
{
    public class HelpCommand : ICommand
    {
        private readonly Action _usage;

        public HelpCommand(Action usage)
        {
            _usage = usage;
        }

        public string Name() => "help";

        public string Description() => "help [command]           display help for command";

        public int Execute(string[] args)
        {
            _usage();
            return Cli.ExitSuccess;
        }
    }
}
