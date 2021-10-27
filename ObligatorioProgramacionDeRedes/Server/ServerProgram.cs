using System;
using System.Threading;
using System.Threading.Tasks;
using Server;

namespace Server
{
    class ServerProgram
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Server");
            await HandleConnections();
            
        }

        static async Task HandleConnections()
        {
        
            ConnectionsHandler connectionsHandler = new ConnectionsHandler();
            await connectionsHandler.StartListeningAsync();
            Console.WriteLine("Press any key to shut down the server");
            Console.ReadLine();
            await connectionsHandler.StartShutServerDownAsync();

        }
    }
}