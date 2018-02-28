using System;
using System.Threading.Tasks;

namespace ChatLe.Cryptography.Tools
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 2 || args[0] == "-h" || args[0] == "--help")
            {
                Console.WriteLine("donet-encrypt");
                Console.WriteLine("\tencrypt a file that can be read using ChatLe.Cryptography.Utility.");
                Console.WriteLine("Usage:");
                Console.WriteLine("\tdotnet encrypt <path to the file to encryp> <secret-key>");
                return;
            }

            using (var utility = new Utility(args[1]))
            {
                await utility.EncryptFile(args[0]);
            }
        }
    }
}
