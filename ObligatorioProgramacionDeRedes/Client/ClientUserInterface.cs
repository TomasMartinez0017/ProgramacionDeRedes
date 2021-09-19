﻿using System;
using Domain;
using System.Collections.Specialized;
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

        public void StartClient()
        {
            _connectionsHandler.Connect();
            Console.WriteLine("Connection to Server Started");

            while (_connectionsHandler.IsClientStateUp())
            {
                int option = DeployMenu();

                if (option == -1)
                {
                    _connectionsHandler.ShutDown();
                }
                else
                {
                    Frame request = _requestHandler.BuildRequest(option);
                    
                    Frame response = _connectionsHandler.SendRequestAndGetResponse(request);

                    string data = _responseHandler.ProcessResponse(response);

                    Console.WriteLine(data);

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
            Console.WriteLine("3 - Rate game");
            Console.WriteLine("4 - Publish game");
            Console.WriteLine("5 - Update game");
            Console.WriteLine("6 - Delete game");
            Console.WriteLine("7 - Search game");
            Console.WriteLine("8 - Sign up");
            Console.WriteLine("9 - Log in");

            option = Convert.ToInt32(Console.ReadLine());

            if (option < 0 || option > 9)
            {
                Console.WriteLine("Invalid option");
                option = DeployMenu();
            }
            
            return option - 1; //Comandos arrancan en 0 por eso debemos corregir restando 1
        }

    }
}