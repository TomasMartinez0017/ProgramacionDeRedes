using System;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Text;

namespace Client.Connections
{
    public class ConnectionsHandler
    {
        private TcpClient _tcpClient;
        private IPEndPoint _serverIpEndPoint;
        private ClientState _state;

        public ConnectionsHandler()
        {
            _serverIpEndPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["ServerIP"]), 
                                            Int32.Parse(ConfigurationManager.AppSettings["ServerPort"]));

            _tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["ClientIP"]), 
                    0));
            _state = ClientState.Down;
        }

        public void Connect()
        {
            Console.WriteLine("Trying to connect to server");
            _tcpClient.Connect(_serverIpEndPoint);
            _state = ClientState.Up;
        }

        // public void SendRequest(string word)
        // {
        //
        // }

        public void ShutDown()
        {
            _state = ClientState.ShutingDown;
            _tcpClient.Close();
            _state = ClientState.Down;
        }
        
        public bool IsClientStateUp()
        {
            return _state == ClientState.Up;
        }



    }
}