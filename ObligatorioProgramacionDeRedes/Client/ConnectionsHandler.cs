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
        private TcpClient _tcpClient;
        private IPEndPoint _serverIpEndPoint;
        private IPAddress _serverIpAddress;
        private int _serverPort;
        private ProtocolHandler _protocol;
        private ClientState _state= new ClientState();
        private SemaphoreSlim _clientStateSemaphore;

        public ConnectionsHandler()
        {
            _serverIpAddress = IPAddress.Parse(ConfigurationManager.AppSettings["ServerIP"]);
            _serverPort = Int32.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            _tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["ClientIP"]), 
                0));
            _state = ClientState.Down;
            _clientStateSemaphore = new SemaphoreSlim(1);
            _protocol = new ProtocolHandler(_tcpClient);
        }

        public async Task ConnectAsync()
        {
            Console.WriteLine("Trying to connect to server");
            await _tcpClient.ConnectAsync(_serverIpAddress, _serverPort);
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
            _tcpClient.Close();
            _state = ClientState.Down;
            _clientStateSemaphore.Release();
        }
        
        public bool IsClientStateUp()
        {
            return _state == ClientState.Up;
        }

    }
}