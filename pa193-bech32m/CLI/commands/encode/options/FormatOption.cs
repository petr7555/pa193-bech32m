using System;
using System.Linq;

namespace pa193_bech32m.CLI.commands.encode.options
{
    public class FormatOption : IOption
    {
        private readonly string[] _choices = {"hex", "base64", "binary"};
        public string Flags() => "-f, --format <format>";

        public string Description() =>
            $"format of input data (choices: {string.Join(", ", _choices.Select(choice => $"\"{choice}\""))}, default: \"hex\")";

        public bool IsValidOption(string arg) => arg == "-f" || arg == "--format";
        public bool HasArgument() => true;
        public string Key() => "format";
        public bool IsValidArgument(string arg) => _choices.Contains(arg);
        public string AllowedArgumentHint() => $"Allowed choices are {string.Join(", ", _choices)}.";
    }
}
