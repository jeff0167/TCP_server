using System;
using Csharp_UnitTest;

namespace TCP_server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();

            Console.ReadKey();
        }
    }
}
