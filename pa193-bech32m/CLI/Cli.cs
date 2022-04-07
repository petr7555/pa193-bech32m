using System;
using System.IO;
using System.Linq;
using System.Text;
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

        /* Utf8Encoder that removes invalid characters instead of throwing an EncoderFallbackException */
        private static readonly Encoding Utf8Encoder = Encoding.GetEncoding(
            "UTF-8",
            new EncoderReplacementFallback(string.Empty),
            new DecoderExceptionFallback()
        );

        private readonly IOption HelpOption;

        private readonly IOption[] Options;

        private readonly ICommand[] Commands;

        private int GetPadding()
        {
            var allFlags = Commands.Select(arg => arg.Flags()).ToList();
            allFlags.AddRange(Options.Select(option => option.Flags()));
            return allFlags.Max(flag => flag.Length) + 2;
        }

        public Cli(Stream inputStream, Stream outputStream)
        {
            Console.SetIn(new StreamReader(inputStream));
            Console.SetOut(new StreamWriter(outputStream) {AutoFlush = true});

            Commands = new ICommand[]
            {
                new EncodeCommand(inputStream),
                new HelpCommand(PrintUsage, EncodeCommand.PrintUsage)
            };

            HelpOption = new HelpOption(PrintUsage);

            Options = new[]
            {
                new VersionOption(Version),
                HelpOption
            };
        }

        public Cli() : this(Console.OpenStandardInput(), Console.OpenStandardOutput())
        {
        }

        private void PrintUsage()
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

        private bool ContainsHelp(string[] args)
        {
            return args.Any(arg => HelpOption.IsValidOption(arg));
        }

        private IOption GetOption(string arg)
        {
            return Options.First(option => option.IsValidOption(arg));
        }

        private bool IsValidCommand(string arg)
        {
            return Commands.Any(command => command.Name() == arg);
        }

        private bool HasUnknownOptions(string[] args)
        {
            return args.Where(IsOption).Any(passedOption =>
                !Options.Any(validOption => validOption.IsValidOption(passedOption)));
        }

        private string GetFirstUnknownOption(string[] args)
        {
            return args.Where(IsOption).First(passedOption =>
                !Options.Any(validOption => validOption.IsValidOption(passedOption)));
        }

        /** PUBLIC INTERFACE **/
        public static void PrintError(string error)
        {
            var utf8Text = Utf8Encoder.GetString(Utf8Encoder.GetBytes($"error: {error}"));
            Console.WriteLine(utf8Text);
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

            // Decode
            // TODO propose "decode" user interface
            // In case of erroneous decoding suggest the closest valid input.
        }
    }
}
