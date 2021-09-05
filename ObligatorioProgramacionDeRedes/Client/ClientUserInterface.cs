using System;
using Client.Connections;
using Protocol;

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
                int option = DeployMenu();

                if (option == 0)
                {
                    _connectionsHandler.ShutDown();
                }
                else
                {
                    //Build request and send to server
                    //Frame request = BuildRequest(option);
                    //Frame response = _connectionsHandler.SendRequest(request);
                    //Receive response from server
                }
                
                
            }
        }

        private int DeployMenu()
        {
            int option = 0;
            Console.WriteLine("-----VAPOR SYSTEM-----");
            Console.WriteLine("Chose an option:");
            Console.WriteLine("0 - Disconnect form server");
            Console.WriteLine("1 - Show catalog");
            Console.WriteLine("2 - Buy game");
            Console.WriteLine("3 - Rate game");
            Console.WriteLine("4 - Publish game");
            Console.WriteLine("5 - Update game");
            Console.WriteLine("6 - Delete game");
            Console.WriteLine("7 - Search game");
            Console.WriteLine("8 - Disconnect from server");

            option = Convert.ToInt32(Console.ReadLine());

            if (option < 0 || option > 8)
            {
                Console.WriteLine("Invalid option");
                option = DeployMenu();
            }

            return option;

        }
        
    }
}