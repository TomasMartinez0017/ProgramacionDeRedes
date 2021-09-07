using System;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.IO;
using System.Text;
using Client.Connections;
using Protocol;

namespace Client
{
    public class ConnectionsHandler
    {
        private TcpClient _tcpClient;
        private IPEndPoint _serverIpEndPoint;
        private ProtocolHandler _protocol;
        private ClientState _state= new ClientState();

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

        public Frame SendRequestAndGetResponse(Frame request)
        {
            try
            {
                _protocol.Send(request);
                Frame serverResponse = _protocol.Receive();
                return serverResponse;
            }
            catch (IOException)
            {
                Console.WriteLine("Server Down");
                ShutDown();
                return null;
            }
        }

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