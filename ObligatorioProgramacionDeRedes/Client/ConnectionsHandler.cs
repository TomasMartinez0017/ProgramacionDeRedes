using System;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Connections;
using Protocol;

namespace Client
{
    public class ConnectionsHandler
    {
        private IPEndPoint _serverIpEndPoint;

        private ProtocolHandler _protocol;
        private ClientState _state;
        private SemaphoreSlim _clientStateSemaphore;
        private Socket _socketClient;

        public ConnectionsHandler()
        {
            _state = ClientState.Down;
            _clientStateSemaphore = new SemaphoreSlim(1);
            
            _serverIpEndPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["ServerIP"]), 
                Int32.Parse(ConfigurationManager.AppSettings["ServerPort"]));
            IPEndPoint clientIpEndPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["ClientIP"]), 
                0);
            _socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketClient.Bind(clientIpEndPoint);
            _protocol = new ProtocolHandler(_socketClient);
        }

        public async Task ConnectAsync()
        {
            Console.WriteLine("Trying to connect to server");
            await _socketClient.ConnectAsync(_serverIpEndPoint);
            await _clientStateSemaphore.WaitAsync();
            _state = ClientState.Up;
            _clientStateSemaphore.Release();
        }

        public async Task <Frame> SendRequestAndGetResponse(Frame request)
        {
            try
            {
                await _protocol.SendAsync(request);
                Frame serverResponse = await _protocol.ReceiveAsync();
                return serverResponse;
            }
            catch (IOException)
            {
                Console.WriteLine("Server Down");
                await ShutDownAsync();
                return null;
            }
        }

        public async Task ShutDownAsync()
        {
            await _clientStateSemaphore.WaitAsync();
            _state = ClientState.ShutingDown;
            //_tcpClient.Close();
            _socketClient.Shutdown(SocketShutdown.Both);
            _state = ClientState.Down;
            _clientStateSemaphore.Release();
        }
        
        public bool IsClientStateUp()
        {
            return _state == ClientState.Up;
        }

    }
}