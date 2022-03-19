using System;
using System.IO;
using System.Linq;
using pa193_bech32m.CLI.commands.encode;
using pa193_bech32m.CLI.commands.help;
using pa193_bech32m.CLI.options;

namespace pa193_bech32m.CLI
{
    public class Cli
    {
        private const string Version = "0.0.1";
        private const string Description = "Bech32m encoder and decoder";

        private static readonly IOption[] Options =
        {
            new VersionOption(Version),
            new HelpOption(PrintUsage)
        };

        private static readonly ICommand[] Commands =
        {
            new EncodeCommand(),
            new HelpCommand()
        };

        public Cli(TextWriter output)
        {
            Console.SetOut(output);
        }

        public static void ExitWithError(string error)
        {
            Console.WriteLine($"error: {error}");
            Environment.Exit(1);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: bech32m [options] [command]");
            Console.WriteLine();
            Console.WriteLine(Description);
            Console.WriteLine();
            Console.WriteLine("Options:");
            foreach (var option in Options)
            {
                Console.WriteLine($"  {option.Flags().PadRight(25, ' ')}{option.Description()}");
            }

            Console.WriteLine();
            Console.WriteLine("Commands:");
            foreach (var command in Commands)
            {
                Console.WriteLine($"  {command.Description()}");
            }
        }

        private static bool IsOption(string arg) => arg.StartsWith("-") || arg.StartsWith("--");

        private static bool IsValidOption(string arg)
        {
            return Options.Any(option => option.IsValidOption(arg));
        }

        private static IOption GetOption(string arg)
        {
            return Options.First(option => option.IsValidOption(arg));
        }

        private static bool IsValidCommand(string arg)
        {
            return Commands.Any(command => command.Name() == arg);
        }

        public void Run(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                Environment.Exit(1);
            }

            foreach (var arg in args)
            {
                if (IsOption(arg))
                {
                    if (IsValidOption(arg))
                    {
                        var option = GetOption(arg);
                        option.Execute();
                    }
                    else
                    {
                        ExitWithError($"unknown option '{arg}'");
                    }
                }
                else if (IsValidCommand(arg))
                {
                    Commands.First(command => command.Name() == arg).Execute(args[1..]);
                }
                else
                {
                    ExitWithError($"unknown command '{arg}'");
                }
            }

            // CLI interface
            // should be hard to misuse
            // if -o is not specified, stdout is used by default
            // hrp is always passed as ASCII cli argument

            // Encode

            // cli-argument
            // usage: bech32m encode --format <format> <hrp> <data> [-o <outputfile>]
            // eg. bech32m encode --format base64 abc Eu8=
            // eg. bech32m encode --format hex abc 12ef -o result
            // eg. bech32m encode --format binary abc 0001001011101111

            // file
            // usage: bech32m encode --format <format> --file <filename> <hrp> [-o <outputfile>]
            // input file contains only data
            // in case of binary input, the file is binary (it does not contain ascii zeroes and ones)

            // stdin (default)
            // usage: bech32m encode --format <format> <hrp> [-o <outputfile>]
            // waits for user input for data
            // should work with pipe
            // https://stackoverflow.com/a/199534/9290771
            // https://stackoverflow.com/a/1562452/9290771

            // Decode
            // TODO propose "decode" user interface
            // In case of erroneous decoding suggest the closest valid input.
        }
    }
}
