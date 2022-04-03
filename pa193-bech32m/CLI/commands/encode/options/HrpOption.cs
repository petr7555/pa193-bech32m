namespace pa193_bech32m.CLI.commands.encode.options
{
    public class HrpOption : IOption
    {
        public string Flags() => "--hrp <hrp>";
        public string Description() => "human-readable part. Consists of 1–83 ASCII characters in range [33, 126].";
        public bool IsValidOption(string arg) => arg == "--hrp";
        public bool HasArgument() => true;
        public string Key() => "hrp";
    }
}
