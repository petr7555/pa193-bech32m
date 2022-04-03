using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using pa193_bech32m.CLI.commands.encode.arguments;
using pa193_bech32m.CLI.commands.encode.options;
using pa193_bech32m.CLI.options;

namespace pa193_bech32m.CLI.commands.encode
{
    public class EncodeCommand : ICommand
    {
        private Stream _inputStream;

        public EncodeCommand(Stream inputStream)
        {
            _inputStream = inputStream;
        }

        private static readonly IArgument[] Arguments =
        {
            new DataArgument()
        };

        private static readonly IOption HrpOption = new HrpOption();
        private static readonly FormatOption FormatOption = new();
        private static readonly IOption InputFileOption = new InputFileOption();
        private static readonly IOption OutputFileOption = new OutputFileOption();
        private static readonly IOption HelpOption = new HelpOption(PrintUsage);

        private static readonly (IOption option, bool required)[] Options =
        {
            (HrpOption, true),
            (FormatOption, false),
            (InputFileOption, false),
            (OutputFileOption, false),
            (HelpOption, false)
        };

        private static bool ContainsHelp(string[] args)
        {
            return args.Any(arg => HelpOption.IsValidOption(arg));
        }

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

        private static bool HasUnknownOptions(string[] args)
        {
            return args.Where(Cli.IsOption).Any(passedOption =>
                !Options.Any(validOptionPair => validOptionPair.option.IsValidOption(passedOption)));
        }

        private static string GetFirstUnknownOption(string[] args)
        {
            return args.Where(Cli.IsOption).First(passedOption =>
                !Options.Any(validOptionPair => validOptionPair.option.IsValidOption(passedOption)));
        }

        private static bool ArgumentIsMissing(List<string> args)
        {
            return args.Count < Arguments.Length;
        }

        private static string GetMissingArgument(List<string> args)
        {
            return Arguments[args.Count].Flags();
        }

        private static int GetPadding()
        {
            var allFlags = Arguments.Select(arg => arg.Flags()).ToList();
            allFlags.AddRange(Options.Select(entry => entry.option.Flags()));
            return allFlags.Max(flag => flag.Length) + 2;
        }

        public static void PrintUsage()
        {
            var padding = GetPadding();

            Console.WriteLine("Usage: bech32m encode [options] <data>");
            Console.WriteLine();
            Console.WriteLine("encode hrp and data into Bech32m string");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            foreach (var argument in Arguments)
            {
                Console.WriteLine($"  {argument.Flags().PadRight(padding, ' ')}{argument.Description()}");
            }

            Console.WriteLine();
            Console.WriteLine("Options:");
            foreach (var (option, _) in Options)
            {
                Console.WriteLine($"  {option.Flags().PadRight(padding, ' ')}{option.Description()}");
            }
        }

        public string Name() => "encode";
        public string Flags() => "encode [options] <data>";
        public string Description() => "encode hrp and data into Bech32m string";

        /**
         * 1) Verify correctness of passed options
         * 2) Check if any required options are missing
         * 3) Check that there are no unknown options
         * 4) Encode
         */
        public int Execute(string[] args)
        {
            if (ContainsHelp(args))
            {
                return HelpOption.Execute();
            }


            var arguments = new List<string>();
            var options = new Dictionary<string, string>();

            var argsCopy = args;
            while (argsCopy.Length > 0)
            {
                var arg = argsCopy[0];
                argsCopy = argsCopy[1..];

                if (Cli.IsOption(arg))
                {
                    if (IsValidOption(arg))
                    {
                        var option = GetOption(arg);
                        if (option.HasArgument())
                        {
                            if (argsCopy.Length > 0)
                            {
                                var optionArgument = argsCopy[0];
                                argsCopy = argsCopy[1..];

                                if (!option.IsValidArgument(optionArgument))
                                {
                                    Cli.PrintError(
                                        $"option '{option.Flags()}' argument '{optionArgument}' is invalid. {option.AllowedArgumentHint()}");
                                    return Cli.ExitFailure;
                                }

                                options[option.Key()] = optionArgument;
                            }
                            else
                            {
                                Cli.PrintError($"option '{option.Flags()}' argument missing");
                                return Cli.ExitFailure;
                            }
                        }

                        // Otherwise option is a flag option.
                        // There are no flag options for EncodeCommand, therefore this branch is not implemented.
                    }

                    // Ignore invalid options for now, they are checked later.
                }
                else
                {
                    arguments.Add(arg);
                }
            }

            if (!HasRequiredOptions(args))
            {
                var option = GetFirstMissingOption(args);
                Cli.PrintError($"required option '{option.Flags()}' not specified");
                return Cli.ExitFailure;
            }

            if (HasUnknownOptions(args))
            {
                var option = GetFirstUnknownOption(args);
                Cli.PrintError($"unknown option '{option}'");
                return Cli.ExitFailure;
            }

            /* hrp */
            var hrp = options[HrpOption.Key()];
            var (isValid, errorMsg) = Bech32m.ValidateHrp(hrp);
            if (!isValid)
            {
                Cli.PrintError(errorMsg);
                return Cli.ExitFailure;
            }

            var format = options.GetValueOrDefault(FormatOption.Key(), FormatOption.Hex);

            /* data */
            string dataInHex;

            if (options.ContainsKey(InputFileOption.Key()))
            {
                var inputFileName = options[InputFileOption.Key()];
                // TODO add test for this
                if (!File.Exists(inputFileName))
                {
                    Cli.PrintError($"input file {inputFileName} does not exist");
                    return Cli.ExitFailure;
                }

                switch (format)
                {
                    case FormatOption.Binary:
                        dataInHex = Convert.ToHexString(File.ReadAllBytes(inputFileName));
                        break;
                    case FormatOption.Hex:
                        dataInHex = File.ReadAllText(inputFileName);
                        if (!Bech32m.IsValidHexInput(dataInHex))
                        {
                            Cli.PrintError("data are not in hex format");
                            return Cli.ExitFailure;
                        }

                        break;
                    case FormatOption.Base64:
                        try
                        {
                            dataInHex = Convert.ToHexString(Convert.FromBase64String(File.ReadAllText(inputFileName)));
                        }
                        catch (FormatException)
                        {
                            Cli.PrintError("data are not in base64 format");
                            return Cli.ExitFailure;
                        }

                        break;
                    default:
                        throw new ArgumentException("invalid format");
                }
            }
            else if (arguments.Count > 0)
            {
                switch (format)
                {
                    case FormatOption.Binary:
                        Cli.PrintError("binary data cannot be passed as command-line argument");
                        return Cli.ExitFailure;
                    case FormatOption.Hex:
                        dataInHex = arguments[0];
                        if (!Bech32m.IsValidHexInput(dataInHex))
                        {
                            Cli.PrintError("data are not in hex format");
                            return Cli.ExitFailure;
                        }

                        break;
                    case FormatOption.Base64:
                        try
                        {
                            dataInHex = Convert.ToHexString(Convert.FromBase64String(arguments[0]));
                        }
                        catch (FormatException)
                        {
                            Cli.PrintError("data are not in base64 format");
                            return Cli.ExitFailure;
                        }

                        break;
                    default:
                        throw new ArgumentException("invalid format");
                }
            }
            else
            {
                Console.WriteLine($"Enter data in {format} format. Press Enter when done.");
                switch (format)
                {
                    case FormatOption.Binary:
                        IEnumerable<byte> allBytes = Array.Empty<byte>();
                        var buffer = new byte[2048];
                        int bytes;
                        // var reader = new BinaryReader(stdin);
                        while ((bytes = _inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            allBytes = allBytes.Concat(buffer[..bytes]);
                        }

                        // Convert.ToHexString(allBytes.Concat(buffer[..bytes]).ToArray())
                        dataInHex = Convert.ToHexString(allBytes.ToArray());
                        break;
                    case FormatOption.Hex:
                        dataInHex = Console.ReadLine();
                        if (!Bech32m.IsValidHexInput(dataInHex))
                        {
                            Cli.PrintError("data are not in hex format");
                            return Cli.ExitFailure;
                        }

                        break;
                    case FormatOption.Base64:
                        try
                        {
                            dataInHex = Convert.ToHexString(Convert.FromBase64String(Console.ReadLine()));
                        }
                        catch (FormatException)
                        {
                            Cli.PrintError("data are not in base64 format");
                            return Cli.ExitFailure;
                        }

                        break;
                    default:
                        throw new ArgumentException("invalid format");
                }

                Console.WriteLine();
            }

            // TODO validate input data

            var encodedString = Bech32m.Encode(hrp, dataInHex);

            // output result
            if (options.ContainsKey(OutputFileOption.Key()))
            {
                File.WriteAllText(options[OutputFileOption.Key()], encodedString);
            }
            else
            {
                Console.WriteLine("Result:");
                Console.WriteLine(encodedString);
            }

            return Cli.ExitSuccess;
        }
    }
}
