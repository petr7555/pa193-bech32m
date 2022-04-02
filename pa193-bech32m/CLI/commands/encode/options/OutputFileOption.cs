using System;

namespace pa193_bech32m.CLI.commands.encode.options
{
    public class OutputFileOption : IOption
    {
        public string Flags() => "-o, --output <outputfile>";
        public string Description() => "output file where result will be saved. If not present, result is printed to stdout.";
        public bool IsValidOption(string arg) => arg == "-o" || arg == "--output";
        public bool HasArgument() => true;
        public string Key() => "output";
    }
}
