using System;
using Domain;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Client.Connections;
using DataAccess;
using Protocol;

namespace Client
{
    public class ClientUserInterface
    {
        private ConnectionsHandler _connectionsHandler;
        private RequestHandler _requestHandler;
        private ResponseHandler _responseHandler;

        public ClientUserInterface()
        {
            _connectionsHandler = new ConnectionsHandler();
            _requestHandler = new RequestHandler();
            _responseHandler = new ResponseHandler();
        }

        public async Task StartClient()
        {
            await _connectionsHandler.ConnectAsync();
            Console.WriteLine("Connection to Server Started");

            while (_connectionsHandler.IsClientStateUp())
            {
                int option = DeployMenu();

                if (option == -1)
                {
                    await _connectionsHandler.ShutDownAsync();
                }
                else 
                {
                    Frame request = _requestHandler.BuildRequest(option);
                    Frame response = await _connectionsHandler.SendRequestAndGetResponse(request);
                    if (response != null)
                    {
                        string data = _responseHandler.ProcessResponse(response);
                        Console.WriteLine(data);
                    }
                    else
                    {
                        await _connectionsHandler.ShutDownAsync();
                    }
                }
            }
        }

        private int DeployMenu()
        {
            int option = -1;
            Console.WriteLine("-----VAPOR SYSTEM-----");
            Console.WriteLine("Chose an option:");
            Console.WriteLine("0 - Disconnect form server");
            Console.WriteLine("1 - Show catalog");
            Console.WriteLine("2 - Buy game");
            Console.WriteLine("3 - Create review");
            Console.WriteLine("4 - Publish game");
            Console.WriteLine("5 - Update game");
            Console.WriteLine("6 - Delete game");
            Console.WriteLine("7 - Search game");
            Console.WriteLine("8 - Show game reviews");
            Console.WriteLine("9 - Upload/Update game cover");
            Console.WriteLine("10 - Download game cover");
            Console.WriteLine("11 - Sign up");
            Console.WriteLine("12 - Log in");

            string optionSelected = Console.ReadLine();
            if (OptionIsNumeric(optionSelected) && !String.IsNullOrEmpty(optionSelected))
            {
                option = Convert.ToInt32(optionSelected);
                if (option < 0 || option > 12)
                {
                    Console.WriteLine("ERROR: Invalid option.\n");
                    option = DeployMenu();
                }
                return option - 1; //Comandos arrancan en 0 por eso debemos corregir restando 1
            }
            else
            {
                Console.WriteLine("ERROR: Invalid option.\n");
                return DeployMenu();
            }
        }
        private bool OptionIsNumeric(string option)
        {
          Regex onlyNumbers =  new Regex("^[0-9]*$");
          return onlyNumbers.IsMatch(option);
        }
    }
   
}