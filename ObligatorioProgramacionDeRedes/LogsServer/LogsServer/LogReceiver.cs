using RabbitMQ.Client;
using System;
using LogsServer.DataAccess;
using RabbitMQ.Client.Events;
using System.Text;
using LogsServer.Domain;
using System.Configuration;

namespace LogsServer
{
    public class LogReceiver
    {
        private IModel _channel;
        private string _queueName;
        private LogProcessor _logProcessor;
        private LogRepository _logRepository;

        public LogReceiver()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory() 
            {
                HostName = ConfigurationManager.AppSettings["RabbitMQServerIP"],
                Port = Int32.Parse(ConfigurationManager.AppSettings["RabbitMQServerPort"])
            };
            IConnection connection = connectionFactory.CreateConnection(); 
            _queueName = ConfigurationManager.AppSettings["LogsQueueName"];
            _channel = connection.CreateModel();                          
            _channel.QueueDeclare(_queueName, false, false, false, null);
            _logRepository = LogRepository.GetInstance();
            _logProcessor = new LogProcessor();
        }

        public void ReceiveServerLogs()
        {
            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);       //  - definimos como consumimos los mensajes
            consumer.Received += async (sender, eventArgs) =>
            {
                byte[] body = eventArgs.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                Log processedLog = _logProcessor.ProcessLog(message);

                await _logRepository.StoreAsync(processedLog);
            };

            _channel.BasicConsume(_queueName, true, consumer);
        }
    }
}