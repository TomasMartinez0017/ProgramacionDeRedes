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

            HandleClient();
        }

        public static void HandleClient()
        {
        
            ConnectionsHandler connectionsHandler = new ConnectionsHandler();
            connectionsHandler.StartListening();
            
            /* Como hago esto?
            Console.WriteLine("Press any key to shut down the server");
            Console.ReadLine();
            connectionsHandler.StartShutServerDown();
            */
       
        }
    }
}