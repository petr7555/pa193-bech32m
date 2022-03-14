using System;

namespace pa193_bech32m
{
    class Program
    {
        static void Main(string[] args)
        {
            // CLI interface
            // if -o is not specified, stdout is used by default
            
            // Encode
            
            // cli-argument
            // usage: bech32m encode --format <format> <hrp> <data> [-o <outputfile>]
            // eg. bech32m encode --format base64 YXZj Eu8=
            // eg. bech32m encode --format hex 616263 12ef -o result
            // eg. bech32m encode --format binary 011000010110001001100011 0001001011101111
            
            // file
            // usage: bech32m encode --format <format> --file <filename> [-o <outputfile>]
            // input file contains hrp on the first line and data on the second line

            // stdin (default)
            // usage: bech32m encode --format <format> [-o <outputfile>]
            // waits for user input for hrp
            // enter
            // waits for user input for data

            // Decode
            // TODO propose "decode" user interface
            // In case of erroneous decoding suggest the closest valid input.
            
            var res = Bech32m.Encode("abc", "751e76e8199196d454941c45d1b3a323f1433bd6");
            Console.WriteLine(res);
        }
    }
}
