using System;
using System.Threading;
using Server.Connections;

namespace Server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server");
            HandleConnections();
            
        }

        public static void HandleConnections()
        {
        
            ConnectionsHandler connectionsHandler = new ConnectionsHandler();
            Thread listeningThread = new Thread(() => connectionsHandler.StartListening());
            listeningThread.Start();
            
            Console.WriteLine("Press any key to shut down the server");
            Console.ReadLine();
            connectionsHandler.StartShutServerDown();

        }
    }
}