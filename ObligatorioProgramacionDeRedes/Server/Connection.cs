using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Text;
using CustomExceptions;
using DataAccess;
using Domain;
using Protocol;

namespace Server
{
    public class Connection
    {
        private User _userConnected;
        private TcpClient _tcpClient;
        private ConnectionState _state;
        private ProtocolHandler _protocol;
        private ResponseHandler _responseHandler;

        public Connection(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _state = ConnectionState.Down;
            _protocol = new ProtocolHandler(tcpClient);
            _responseHandler = new ResponseHandler();
            _userConnected = new User();
        }

        public void StartConnection()
        {
            _state = ConnectionState.Up;
            while (_state == ConnectionState.Up)
            {
                HandleRequests();
            }
        }

        public void ShutDown()
        {
            _tcpClient.Close();
            _state = ConnectionState.Down;
            ActiveUserRepository repository = ActiveUserRepository.GetInstance();
            repository.DisconnectAlUsers();
            
        }

        private void HandleRequests()
        {
            try
            {
                ActiveUserRepository activeUserRepository = ActiveUserRepository.GetInstance();
                List<User> usersConnected = activeUserRepository.GetUsers();
            
                Frame request = _protocol.Receive();
                Frame response = _responseHandler.GetResponse(request, usersConnected);

                ManageSignUp(request, response, activeUserRepository);

                ManageLogIn(request, response, activeUserRepository);
            
                _protocol.Send(response);
            }
            catch (ClientExcpetion e)
            {
                Console.WriteLine(e.Message);
                ShutDown();
            }
        }

        private void ManageLogIn(Frame request, Frame response, ActiveUserRepository activeUserRepository)
        {
            if ((Command) request.Command == Command.LogIn && (Status) response.Status == Status.Ok)
            {
                UserRepository userRepository = UserRepository.GetInstance();
                _userConnected = userRepository.GetUser(Encoding.UTF8.GetString(response.Data));
                activeUserRepository.AddUser(_userConnected);
                response.Data = Encoding.UTF8.GetBytes("User logged in successfully. Welcome to VAPOR SYSTEM");
                response.DataLength = response.Data.Length;
            }
        }

        private void ManageSignUp(Frame request, Frame response, ActiveUserRepository activeUserRepository)
        {
            if ((Command) request.Command == Command.SignUp && (Status) response.Status == Status.Ok)
            {
                _userConnected.Username = Encoding.UTF8.GetString(response.Data);
                activeUserRepository.AddUser(_userConnected);
                response.Data = Encoding.UTF8.GetBytes("User created successfully. Welcome to VAPOR SYSTEM");
                response.DataLength = response.Data.Length;
            }
        }
    }
}