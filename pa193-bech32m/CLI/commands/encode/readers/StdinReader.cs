using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            var result = ReadTextDataFromStdin();
            Console.WriteLine();
            return result;
        }

        public (bool hasError, byte[] data) ReadBytes()
        {
            Prompt();
            var result = ReadBinaryDataFromStdin();
            Console.WriteLine();
            return result;
        }

        private void Prompt()
        {
            Console.WriteLine($"Enter data in {_format} format. Press Enter when done.");
        }

        private (bool hasError, string data) ReadTextDataFromStdin() => ReadDataFromStdinWithErrorHandling(
            maxBufferSize => new StreamReader(_inputStream, Encoding.Default, true, maxBufferSize).ReadLine() ?? "",
            "");

        private (bool hasError, byte[] data) ReadBinaryDataFromStdin() =>
            ReadDataFromStdinWithErrorHandling(maxBufferSize =>
            {
                IEnumerable<byte> allBytes = Array.Empty<byte>();
                var buffer = new byte[maxBufferSize];
                int bytes;
                while ((bytes = _inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    allBytes = allBytes.Concat(buffer[..bytes]);
                }

                return allBytes.ToArray();
            }, Array.Empty<byte>());

        private static (bool hasError, T data) ReadDataFromStdinWithErrorHandling<T>(Func<int, T> readFn,
            T defaultValue)
        {
            /*
             * By default, Stream.Read(), StreamReader, Console.Readline() etc. can read only 1024 characters at once.
             * This is an issue when long data are passed all at once from command-line.
             * It is not an issue when the data are piped (e.g. using echo) or directly written to underlying stream in tests.
             * As a consequence, the following code is impossible to unit test and is tested only manually.
             *
             * The default buffer size limit (1024) is increased.
             * It is not, however, possible to support unlimited size of directly passed input,
             * therefore the exception is caught and an alternative is offered.
             */

            const int maxBufferSize = 4096;
            try
            {
                return (false, readFn(maxBufferSize));
            }
            catch (ArgumentException)
            {
                Cli.PrintError(
                    $"Cannot read more than {maxBufferSize} characters at once. Use pipe to pass the data or the \"--input\" option.");
                return (true, defaultValue);
            }
        }
    }
}
