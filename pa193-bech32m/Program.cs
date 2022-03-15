using System;

namespace pa193_bech32m
{
    class Program
    {
        static void Main(string[] args)
        {
            // CLI interface
            // should be hard to misuse
            // if -o is not specified, stdout is used by default
            // hrp is always passed as ASCII cli argument
            
            // Encode
            
            // cli-argument
            // usage: bech32m encode --format <format> <hrp> <data> [-o <outputfile>]
            // eg. bech32m encode --format base64 abc Eu8=
            // eg. bech32m encode --format hex abc 12ef -o result
            // eg. bech32m encode --format binary abc 0001001011101111
            
            // file
            // usage: bech32m encode --format <format> --file <filename> <hrp> [-o <outputfile>]
            // input file contains only data
            // in case of binary input, the file is binary (it does not contain ascii zeroes and ones)

            // stdin (default)
            // usage: bech32m encode --format <format> <hrp> [-o <outputfile>]
            // waits for user input for data
            // should work with pipe
            // https://stackoverflow.com/a/199534/9290771
            // https://stackoverflow.com/a/1562452/9290771

            // Decode
            // TODO propose "decode" user interface
            // In case of erroneous decoding suggest the closest valid input.
            
            var res = Bech32m.Encode("abc", "751e76e8199196d454941c45d1b3a323f1433bd6");
            Console.WriteLine(res);
        }
    }
}
