using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private Socket _socketClient;
        private ConnectionState _state;
        private ProtocolHandler _protocol;
        private ResponseHandler _responseHandler;
        private SemaphoreSlim _connectionStateSempahore;
        private SemaphoreSlim _receiveDataSemaphore;

        public Connection(Socket socketClient)
        {
            _socketClient = socketClient;
            _state = ConnectionState.Down;
            _protocol = new ProtocolHandler(_socketClient);
            _responseHandler = new ResponseHandler();
            _userConnected = new User();
            _connectionStateSempahore = new SemaphoreSlim(1);
            _receiveDataSemaphore = new SemaphoreSlim(1);
        }

        public async Task StartConnectionAsync()
        {
            _state = ConnectionState.Up;
            while (_state == ConnectionState.Up)
            {
               await HandleRequestsAsync();
            }
        }

        public async Task ShutDownAsync()
        {
            _socketClient.Shutdown(SocketShutdown.Both);
            ActiveUserRepository repository = ActiveUserRepository.GetInstance();
            await repository.DisconnectUserAsync(_userConnected);
            await _connectionStateSempahore.WaitAsync();
            _state = ConnectionState.Down;
            _connectionStateSempahore.Release();
            
        }

        private async Task HandleRequestsAsync()
        {
            try
            {
                ActiveUserRepository activeUserRepository = ActiveUserRepository.GetInstance();
                List<User> usersConnected = await activeUserRepository.GetUsersAsync();
                
                Frame request = await _protocol.ReceiveAsync();
                Frame response = await _responseHandler.GetResponseAsync(request, usersConnected, _userConnected);

                await ManageSignUp(request, response, activeUserRepository); //Debería ser un Async Task?

                await ManageLogIn(request, response, activeUserRepository); //Debería ser un Async Task?

                await _protocol.SendAsync(response);
            }
            catch (ClientExcpetion e)
            {
                Console.WriteLine(e.Message);
                await ShutDownAsync();
            }
            catch (IOException e)
            {
                Console.WriteLine("Server terminated connection to client");
            }
        }

        private async Task ManageLogIn(Frame request, Frame response, ActiveUserRepository activeUserRepository)
        {
            if ((Command) request.Command == Command.LogIn && (FrameStatus) response.Status == FrameStatus.Ok)
            {
                UserRepository userRepository = UserRepository.GetInstance();
                _userConnected = await userRepository.GetUserAsync(Encoding.UTF8.GetString(response.Data));
                await activeUserRepository.AddUserAsync(_userConnected);
                response.Data = Encoding.UTF8.GetBytes("User logged in successfully. Welcome to VAPOR SYSTEM");
                response.DataLength = response.Data.Length;
            }
        }

        private async Task ManageSignUp(Frame request, Frame response, ActiveUserRepository activeUserRepository)
        {
            if ((Command) request.Command == Command.SignUp && (FrameStatus) response.Status == FrameStatus.Ok)
            {
                _userConnected.Username = Encoding.UTF8.GetString(response.Data);
                await activeUserRepository.AddUserAsync(_userConnected);
                response.Data = Encoding.UTF8.GetBytes("User created successfully. Welcome to VAPOR SYSTEM");
                response.DataLength = response.Data.Length;
            }
        }
    }
}