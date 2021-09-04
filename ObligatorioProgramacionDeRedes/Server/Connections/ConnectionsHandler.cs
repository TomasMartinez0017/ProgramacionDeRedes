using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;

namespace Server.Connections
{
    public class ConnectionsHandler
    {
        private IPAddress _serverIp;
        private int _serverPort;
        private TcpListener _tcpListener;
        private List<Connection> _connections;
        private ServerState _serverState;
        private object _serverStateLocker;
        private object _connectionsListLocker;

        public ConnectionsHandler()
        {
             _serverStateLocker = new object();
            _connectionsListLocker = new object();
            _connections = new List<Connection>();
            _serverIp= IPAddress.Parse(ConfigurationManager.AppSettings["ServerIP"]);
            _serverPort= Int32.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            _serverState = ServerState.Down;
            _tcpListener = new TcpListener(_serverIp, _serverPort);
        }

        public void StartListening()
        {
            _tcpListener.Start(10);
            _serverState = ServerState.Up;
            while (IsServerUp())
            {
              try
              {
                    _tcpListener.AcceptTcpClient();
                    Console.WriteLine("Accepted new client connection");
              }
              catch (SocketException se)
              {
                    Console.WriteLine($"The client connection was interrupted, message {se.Message}");
              } 
            }

        }

        public void StartShutServerDown()
        {
            lock (_serverStateLocker)
            {
              _serverState=ServerState.ShutingDown;
              _tcpListener.Stop();
            }
        }

        public bool IsServerUp()
        {
            lock (_serverStateLocker)
            {
             return _serverState == ServerState.Up;
            }
        }
    }
}