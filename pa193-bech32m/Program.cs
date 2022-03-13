using System;

namespace pa193_bech32m
{
    class Program
    {
        static void Main(string[] args)
        {
            var res = Bech32m.Encode("abc", "751e76e8199196d454941c45d1b3a323f1433bd6");
            Console.WriteLine(res);
        }
    }
}
