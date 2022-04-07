using System;
using System.IO;

namespace pa193_bech32m.CLI.commands.encode.readers
{
    internal class FileReader : IReader
    {
        private readonly string _inputFileName;

        public FileReader(string inputFileName)
        {
            _inputFileName = inputFileName;
        }

        private bool FileExists()
        {
            if (!File.Exists(_inputFileName))
            {
                Cli.PrintError($"input file {_inputFileName} does not exist");
                return false;
            }

            return true;
        }

        public (bool hasError, string data) ReadString()
        {
            if (!FileExists())
            {
                return (true, "");
            }

            var fileContents = File.ReadAllText(_inputFileName);
            var fileContentsWithoutNewlines = fileContents.Replace("\r", "").Replace("\n", "");
            return (false, fileContentsWithoutNewlines);
        }

        public (bool hasError, byte[] data) ReadBytes()
        {
            if (!FileExists())
            {
                return (true, Array.Empty<byte>());
            }

            return (false, File.ReadAllBytes(_inputFileName));
        }
    }
}
