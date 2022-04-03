using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pa193_bech32m.CLI.commands.encode.readers
{
    internal class StdinReader : IReader
    {
        private readonly Stream _inputStream;
        private readonly string _format;

        public StdinReader(Stream inputStream, string format)
        {
            _inputStream = inputStream;
            _format = format;
        }

        public (bool hasError, string data) ReadString()
        {
            Prompt();
            var data = Console.ReadLine() ?? "";
            Console.WriteLine();
            return (false, data);
        }

        public (bool hasError, byte[] data) ReadBytes()
        {
            Prompt();
            var binaryData = ReadBinaryDataFromStdin();
            Console.WriteLine();
            return (false, binaryData);
        }

        private void Prompt()
        {
            Console.WriteLine($"Enter data in {_format} format. Press Enter when done.");
        }

        private byte[] ReadBinaryDataFromStdin()
        {
            IEnumerable<byte> allBytes = Array.Empty<byte>();
            var buffer = new byte[2048];
            int bytes;
            while ((bytes = _inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                allBytes = allBytes.Concat(buffer[..bytes]);
            }

            return allBytes.ToArray();
        }
    }
}
