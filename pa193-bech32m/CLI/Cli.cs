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
        public const int ExitSuccess = 0;
        public const int ExitFailure = 1;
        private const string Version = "0.0.1";
        private const string Description = "Bech32m encoder and decoder";

        private static readonly IOption HelpOption = new HelpOption(PrintUsage);

        private static readonly IOption[] Options =
        {
            new VersionOption(Version),
            HelpOption
        };

        private static readonly ICommand[] Commands =
        {
            new EncodeCommand(),
            new HelpCommand(PrintUsage, EncodeCommand.PrintUsage)
        };

        private static int GetPadding()
        {
            var allFlags = Commands.Select(arg => arg.Flags()).ToList();
            allFlags.AddRange(Options.Select(option => option.Flags()));
            return allFlags.Max(flag => flag.Length) + 2;
        }

        public Cli(TextWriter output)
        {
            Console.SetOut(output);
        }

        private static void PrintUsage()
        {
            var padding = GetPadding();

            Console.WriteLine("Usage: bech32m [options] [command]");
            Console.WriteLine();
            Console.WriteLine(Description);
            Console.WriteLine();
            Console.WriteLine("Options:");
            foreach (var option in Options)
            {
                Console.WriteLine($"  {option.Flags().PadRight(padding, ' ')}{option.Description()}");
            }

            Console.WriteLine();
            Console.WriteLine("Commands:");
            foreach (var command in Commands)
            {
                Console.WriteLine($"  {command.Flags().PadRight(padding, ' ')}{command.Description()}");
            }
        }

        private static bool ContainsHelp(string[] args)
        {
            return args.Any(arg => HelpOption.IsValidOption(arg));
        }

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

        private static bool HasUnknownOptions(string[] args)
        {
            return args.Where(IsOption).Any(passedOption =>
                !Options.Any(validOption => validOption.IsValidOption(passedOption)));
        }

        private static string GetFirstUnknownOption(string[] args)
        {
            return args.Where(IsOption).First(passedOption =>
                !Options.Any(validOption => validOption.IsValidOption(passedOption)));
        }

        /** PUBLIC INTERFACE **/
        public static void PrintError(string error)
        {
            Console.WriteLine($"error: {error}");
        }

        public static bool IsOption(string arg) => arg.StartsWith("-") || arg.StartsWith("--");

        public int Run(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                return ExitFailure;
            }

            var arg = args[0];

            if (IsValidCommand(arg))
            {
                return Commands.First(command => command.Name() == arg).Execute(args[1..]);
            }

            if (ContainsHelp(args))
            {
                return HelpOption.Execute();
            }

            if (HasUnknownOptions(args))
            {
                var option = GetFirstUnknownOption(args);
                PrintError($"unknown option '{option}'");
                return ExitFailure;
            }

            if (IsOption(arg))
            {
                var option = GetOption(arg);
                return option.Execute();
            }

            PrintError($"unknown command '{arg}'");
            return ExitFailure;

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
