using System.Data;
using System.Net.Sockets;
using Domain;
using Protocol;

namespace Server.Connections
{
    public class Connection
    {
        private TcpClient _tcpClient;
        private ConnectionState _state;
        private ProtocolHandler _protocol;
        private ResponseHandler _responseHandler;

        public Connection(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _state = ConnectionState.Down;
            _protocol = new ProtocolHandler(tcpClient);
        }

        public void StartConnection()
        {
            _state = ConnectionState.Up;
            while (_state == ConnectionState.Up)
            {
                HandleRequests();
            }
        }

        public void ShutDown()
        {
            _tcpClient.Close();
        }

        private void HandleRequests()
        {
            Frame request = _protocol.Receive();
            //Frame response = _responseHandler.GetResponse(request);
        }

    }
}