using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class ConnectionsHandler
    {
        private IPAddress _serverIp;
        private int _serverPort;
        private List<Connection> _connections;
        private TcpListener _tcpListener;
        private ServerState _serverState;

        public ConnectionsHandler()
        {
            _serverIp= IPAddress.Parse(ConfigurationManager.AppSettings["ServerIP"]);
            _serverPort= Int32.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            _connections = new List<Connection>();
            _serverState = ServerState.Down;
            _tcpListener = new TcpListener(_serverIp, _serverPort);
        }

        public void StartListening()
        {
            _tcpListener.Start(1);
            _serverState = ServerState.Up;
            
            while (IsServerUp())
            {
                try
                {
                    Connection clientConnection = new Connection(_tcpListener.AcceptTcpClient());
                    Thread clientThread = new Thread(() => clientConnection.StartConnection());
                    AddConnection(clientConnection);
                    clientThread.Start();
                    Console.WriteLine("Accepted new client connection");
                }
                catch (SocketException)
                {
                    Console.WriteLine("Server shutting down");
                    ShutDownConnections();
                    _serverState = ServerState.Down;
                }
                
            }
        }

        private void AddConnection(Connection clientConnection) 
        {
            _connections.Add(clientConnection);
        }

        public void ShutDownConnections()
        {
            for (int i = _connections.Count - 1; i >= 0; i--)
            {
                try
                {
                    Connection connection = _connections.ElementAt(i);
                    connection.ShutDown();
                    _connections.RemoveAt(i);
                }
                catch (Exception)
                {
                    Console.WriteLine("Connection already closed");
                }
            }
        }
        
        public void StartShutServerDown()
        {
            _serverState = ServerState.ShutingDown;
            _tcpListener.Stop();
        }

        public bool IsServerUp()
        {
            return _serverState == ServerState.Up;
        }
    }
}