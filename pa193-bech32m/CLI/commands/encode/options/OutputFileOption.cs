using System;

namespace pa193_bech32m.CLI.commands.encode.options
{
    public class OutputFileOption : IOption
    {
        public string Flags() => "-o, --out <outputfile>";
        public string Description() => "output file where result will be saved";
        public bool IsValidOption(string arg) => arg == "-o" || arg == "--out";
        public bool HasArgument() => true;
        public string Key() => "out";

        public int Execute()
        {
            throw new NotImplementedException();
        }
    }
}
