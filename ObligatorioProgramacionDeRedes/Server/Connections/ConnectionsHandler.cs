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
        private ServerState _serverState;

        public ConnectionsHandler()
        {
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
                _tcpListener.AcceptTcpClient();
                Console.WriteLine("Accepted new client connection");
            }
        }

        public void StartShutServerDown()
        {
            _serverState=ServerState.ShutingDown;
            _tcpListener.Stop();
        }

        public bool IsServerUp()
        {
            return _serverState == ServerState.Up;
        }
    }
}