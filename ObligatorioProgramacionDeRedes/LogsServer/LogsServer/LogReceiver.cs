using RabbitMQ.Client;
using System;
using LogsServer.DataAccess;
using RabbitMQ.Client.Events;
using System.Text;
using LogsServer.Domain;

namespace LogsServer
{
    public class LogReceiver
    {
        private IModel _channel;
        private string _queueName;
        private LogProcessor _logProcessor;
        private LogRepository _logRepository;

        public LogReceiver(LogServerConfiguration configuration)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory() 
            {
                HostName = configuration.RabbitMQServerIP,
                Port = Int32.Parse(configuration.RabbitMQServerPort)
            };
            IConnection connection = connectionFactory.CreateConnection(); 
            _queueName = configuration.LogsQueueName;
            _channel = connection.CreateModel();                          
            _channel.QueueDeclare(_queueName, false, false, false, null);
            _logRepository = LogRepository.GetInstance();
            _logProcessor = new LogProcessor();
        }

        public void ReceiveServerLogs()
        {
            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);       // 5 - definimos como consumimos los mensajes
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