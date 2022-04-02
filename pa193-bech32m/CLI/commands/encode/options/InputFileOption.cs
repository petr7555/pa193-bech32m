using System;

namespace pa193_bech32m.CLI.commands.encode.options
{
    public class InputFileOption : IOption
    {
        public string Flags() => "-i, --input <inputfile>";
        public string Description() => "input file with data";
        public bool IsValidOption(string arg) => arg == "-i" || arg == "--input";
        public bool HasArgument() => true;
        public string Key() => "input";

        public int Execute()
        {
            throw new NotImplementedException();
        }
    }
}
