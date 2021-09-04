using System;
using Client.Connections;

namespace Client
{
    public class ClientUserInterface
    {
        private ConnectionsHandler _connectionsHandler;

        public ClientUserInterface()
        {
            _connectionsHandler = new ConnectionsHandler();
        }

        public void StartClient()
        {
            _connectionsHandler.Connect();
            Console.WriteLine("Connection to Server Started");
            
            while (_connectionsHandler.IsClientStateUp())
            {
                Console.WriteLine("Press enter key to end client");

                string option = Console.ReadLine();
                
                _connectionsHandler.ShutDown();
            }
        }
        
    }
}