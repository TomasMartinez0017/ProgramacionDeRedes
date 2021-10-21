using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ConnectionsHandler
    {
        private IPAddress _serverIp;
        private int _serverPort;
        private List<Connection> _connections;
        private TcpListener _tcpListener;
        private ServerState _serverState;
        private SemaphoreSlim _connectionsListSemaphore;
        private SemaphoreSlim _serverStateSemaphore;

        public ConnectionsHandler()
        {
            _serverIp= IPAddress.Parse(ConfigurationManager.AppSettings["ServerIP"]);
            _serverPort= Int32.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            _connections = new List<Connection>();
            _serverState = ServerState.Down;
            _tcpListener = new TcpListener(_serverIp, _serverPort);
            _serverStateSemaphore = new SemaphoreSlim(1);
            _connectionsListSemaphore = new SemaphoreSlim(1);
        }

        public async Task StartListeningAsync()
        {
            _tcpListener.Start(1);
            _serverState = ServerState.Up;
            
            while (IsServerUp())
            {
                try
                {
                    Connection clientConnection = new Connection(_tcpListener.AcceptTcpClient());
                    clientConnection.StartConnectionAsync();
                    //Thread clientThread = new Thread(() => clientConnection.StartConnection());
                    await AddConnectionAsync(clientConnection);
                    //clientThread.Start();
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
            _tcpListener.Stop();
            _serverStateSemaphore.Release();
        }

        private bool IsServerUp()
        {
            return _serverState == ServerState.Up;
        }
    }
}