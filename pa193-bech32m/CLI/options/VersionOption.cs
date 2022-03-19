using System;

namespace pa193_bech32m.CLI.options
{
    public class VersionOption : IOption
    {
        private readonly string _version;

        public VersionOption(string version)
        {
            _version = version;
        }

        public string Description() => "output the version number";
        public string Flags() => "-V, --version";
        public bool IsValidOption(string arg) => arg == "-V" || arg == "--version";

        public void Execute()
        {
            Console.WriteLine(_version);
        }
    }
}
