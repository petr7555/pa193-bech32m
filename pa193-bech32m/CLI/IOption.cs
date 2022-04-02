using System;

namespace pa193_bech32m.CLI
{
    public interface IOption
    {
        public string Flags();
        public string Description();
        public bool IsValidOption(string arg);
        public bool HasArgument();
        public string Key() => "";
        public bool IsValidArgument(string arg) => true;
        public string AllowedArgumentHint() => "";
        public int Execute() => throw new NotImplementedException();
    }
}
