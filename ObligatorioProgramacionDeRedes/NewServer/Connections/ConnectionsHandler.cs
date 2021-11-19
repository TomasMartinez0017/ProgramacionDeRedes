using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NewServer
{
    public class ConnectionsHandler
    {
        private List<Connection> _connections;
        private ServerState _serverState;
        private SemaphoreSlim _connectionsListSemaphore;
        private SemaphoreSlim _serverStateSemaphore;
        private Socket _serverSocket;

        public ConnectionsHandler(ServerConfiguration configuration)
        {
            _connections = new List<Connection>();
            _serverState = ServerState.Down;
            _serverStateSemaphore = new SemaphoreSlim(1);
            _connectionsListSemaphore = new SemaphoreSlim(1);

            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverIpEndPoint = new IPEndPoint(IPAddress.Parse(configuration.ServerIP), 
                Int32.Parse(configuration.ServerPort));
            _serverSocket.Bind(serverIpEndPoint);
        }

        public async Task StartListeningAsync()
        {
            _serverSocket.Listen(1);
            _serverState = ServerState.Up;
            
            while (IsServerUp())
            {
                try
                {
                    Connection clientConnection = new Connection(await _serverSocket.AcceptAsync());
                    clientConnection.StartConnectionAsync();
                    await AddConnectionAsync(clientConnection);
                    Console.WriteLine("Accepted new client connection");
                }
                catch (SocketException)
                {
                    await ShutDownConnectionsAsync();
                }
            }
        }

        private async Task AddConnectionAsync(Connection clientConnection) 
        {
            await _connectionsListSemaphore.WaitAsync();
            _connections.Add(clientConnection);
            _connectionsListSemaphore.Release();
        }

        private async Task ShutDownConnectionsAsync()
        {
            await _serverStateSemaphore.WaitAsync();
            _serverState = ServerState.Down;
            _serverStateSemaphore.Release();
            
            await _connectionsListSemaphore.WaitAsync();
            for (int i = _connections.Count - 1; i >= 0; i--)
            {
                Connection connection = _connections.ElementAt(i);
                await connection.ShutDownAsync();
                _connections.RemoveAt(i);
            }
            _connectionsListSemaphore.Release();
        }
        
        public async Task StartShutServerDownAsync()
        {
            await _serverStateSemaphore.WaitAsync();
            _serverState = ServerState.ShutingDown;
            _serverSocket.Shutdown(SocketShutdown.Both);
            _serverStateSemaphore.Release();
        }

        private bool IsServerUp()
        {
            return _serverState == ServerState.Up;
        }
    }
}