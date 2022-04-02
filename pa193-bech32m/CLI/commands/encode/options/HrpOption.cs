using System;

namespace pa193_bech32m.CLI.commands.encode.options
{
    public class HrpOption : IOption
    {
        public string Flags() => "--hrp <hrp>";
        public string Description() => "human-readable part";
        public bool IsValidOption(string arg) => arg == "--hrp";
        public bool HasArgument() => true;

        public string Key() => "hrp";

        // TODO hrp validation
        public bool IsValidArgument(string arg) => true;

        // TODO hrp hint
        public string AllowedArgumentHint() => "";
    }
}
