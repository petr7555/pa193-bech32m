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

        public string Flags() => "-V, --version";
        public string Description() => "output the version number";
        public bool IsValidOption(string arg) => arg == "-V" || arg == "--version";
        public bool HasArgument() => false;

        public int Execute()
        {
            Console.WriteLine(_version);
            return Cli.ExitSuccess;
        }
    }
}
