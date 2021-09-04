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
                //pruebas
                Console.WriteLine("Press any key to shut down connection");
                Console.ReadLine();
                _connectionsHandler.ShutDown();
            }
        }
        
    }
}