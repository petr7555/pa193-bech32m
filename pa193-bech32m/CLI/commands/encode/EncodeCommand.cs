using System;
using System.Linq;
using pa193_bech32m.CLI.commands.encode.options;

namespace pa193_bech32m.CLI.commands.encode
{
    public class EncodeCommand : ICommand
    {
        private static readonly (HrpOption option, bool required)[] Options = {(new HrpOption(), true)};

        public string Name() => "encode";

        public string Description() => "encode [options] <data>  encode hrp and data into Bech32m string";

        private static bool IsValidOption(string arg)
        {
            return Options.Any(optionPair => optionPair.option.IsValidOption(arg));
        }

        private static bool HasRequiredOptions(string[] args)
        {
            return Options.All(optionPair =>
            {
                var (option, required) = optionPair;
                return !required || args.Any(arg => option.IsValidOption(arg));
            });
        }
        
        private static IOption GetFirstMissingOption(string[] args)
        {
            return Options.First(optionPair =>
            {
                var (option, required) = optionPair;
                return required && args.All(arg => !option.IsValidOption(arg));
            }).option;
        }


        public void Execute(string[] args)
        {
            if (!HasRequiredOptions(args))
            {
                var option = GetFirstMissingOption(args);
                Cli.ExitWithError($"required option '{option.Flags()}' not specified");
            }
            Console.WriteLine(args);
        }
    }
}
