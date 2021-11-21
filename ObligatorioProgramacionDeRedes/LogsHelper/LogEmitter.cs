using System;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using LogsServer.Domain;
using RabbitMQ.Client;

namespace LogsHelper
{
    public class LogEmitter
    {
        private IModel _channel;
        private string _queueName;
        
        public LogEmitter()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = "127.0.0.1",
                Port = Int32.Parse("5672")
            };
            IConnection connection = connectionFactory.CreateConnection();
            _queueName = "ServerLogs";
            _channel = connection.CreateModel();
            _channel.QueueDeclare(_queueName, false, false, false, null);
        }

        public void EmitLog(string logMessage, LogTag tag)
        {
            string messageToSend = $"{DateTime.Now.Ticks}#{tag}#{logMessage}";
            byte[] body = Encoding.UTF8.GetBytes(messageToSend);

            _channel.BasicPublish("", _queueName, null, body);
        }
    }
}