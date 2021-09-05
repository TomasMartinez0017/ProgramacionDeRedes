using System.Data;
using System.Net.Sockets;
using Domain;
namespace Server.Connections
{
    public class Connection
    {
        private TcpClient _tcpClient;
        private ConnectionState _state;

        public Connection(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _state = ConnectionState.Down;
        }

        public void StartConnection()
        {
            _state = ConnectionState.Up;
        }

        public void ShutDown()
        {
            _tcpClient.Close();
        }

    }
}