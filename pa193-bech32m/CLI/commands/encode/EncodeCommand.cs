using System;
using System.Linq;
using pa193_bech32m.CLI.commands.encode.arguments;
using pa193_bech32m.CLI.commands.encode.options;
using pa193_bech32m.CLI.options;

namespace pa193_bech32m.CLI.commands.encode
{
    public class EncodeCommand : ICommand
    {
        private static readonly IArgument[] Arguments =
        {
            new DataArgument()
        };

        private static readonly (IOption option, bool required)[] Options =
        {
            (new HrpOption(), true),
            (new FormatOption(), false),
            (new InputFileOption(), true),
            (new OutputFileOption(), true),
            (new HelpOption(PrintUsage), false)
        };

        private static bool IsValidOption(string arg)
        {
            return Options.Any(optionPair => optionPair.option.IsValidOption(arg));
        }

        private static IOption GetOption(string arg)
        {
            return Options.First(optionPair => optionPair.option.IsValidOption(arg)).option;
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

        public static void PrintUsage()
        {
            Console.WriteLine("Usage: bech32m encode [options] <data>");
            Console.WriteLine();
            Console.WriteLine("encode hrp and data into Bech32m string");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            foreach (var argument in Arguments)
            {
                Console.WriteLine($"  {argument.Flags().PadRight(25, ' ')}{argument.Description()}");
            }

            Console.WriteLine();
            Console.WriteLine("Options:");
            foreach (var (option, _) in Options)
            {
                Console.WriteLine($"  {option.Flags().PadRight(25, ' ')}{option.Description()}");
            }
        }

        public string Name() => "encode";
        public string Flags() => "encode [options] <data>";
        public string Description() => "encode hrp and data into Bech32m string";

        public int Execute(string[] args)
        {
            // if (!HasRequiredOptions(args))
            // {
            //     var option = GetFirstMissingOption(args);
            //     Cli.PrintError($"required option '{option.Flags()}' not specified");
            //     return Cli.ExitFailure;
            // }
            // Console.WriteLine(args);

            var arg = args[0];

            if (Cli.IsOption(arg))
            {
                if (IsValidOption(arg))
                {
                    var option = GetOption(arg);
                    return option.Execute();
                }

                // PrintError($"unknown option '{arg}'");
                // return ExitFailure;
            }

            return Cli.ExitSuccess;
        }
    }
}
