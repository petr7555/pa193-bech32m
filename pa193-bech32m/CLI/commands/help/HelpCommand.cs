using System;
using pa193_bech32m.CLI.commands.encode;

namespace pa193_bech32m.CLI.commands.help
{
    public class HelpCommand : ICommand
    {
        private readonly Action _usage;
        private readonly Action _encodeUsage;

        public HelpCommand(Action usage, Action encodeUsage)
        {
            _usage = usage;
            _encodeUsage = encodeUsage;
        }

        public string Name() => "help";
        public string Flags() => "help [command]";
        public string Description() => "display help for command";

        public int Execute(string[] args)
        {
            if (args.Length == 0)
            {
                _usage();
                return Cli.ExitSuccess;
            }

            switch (args[0])
            {
                case EncodeCommand.CommandName:
                    _encodeUsage();
                    return Cli.ExitSuccess;
                default:
                    _usage();
                    return Cli.ExitFailure;
            }
        }
    }
}
