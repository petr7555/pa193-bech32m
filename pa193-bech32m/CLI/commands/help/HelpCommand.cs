using System;

namespace pa193_bech32m.CLI.commands.help
{
    public class HelpCommand : ICommand
    {
        public string Name() => "help";

        public string Description() => "help [command]           display help for command";

        public int Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
