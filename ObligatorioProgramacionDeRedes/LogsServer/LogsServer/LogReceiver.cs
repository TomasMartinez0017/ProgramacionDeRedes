using RabbitMQ.Client;
using System;
using LogsServer.DataAccess;
using RabbitMQ.Client.Events;
using System.Text;

namespace LogsServer
{
    public class LogReceiver
    {
        private IModel _channel;
        private string _queueName;
        //private LogProcessor _logProcessor;
        private LogRepository _logRepository;

        public LogReceiver(LogServerConfiguration configuration)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory() //1 - Defino la conexion
            {
                HostName = configuration.RabbitMQServerIP,
                Port = Int32.Parse(configuration.RabbitMQServerPort)
            };
            IConnection connection = connectionFactory.CreateConnection(); // 2 - Creamos la conexion
            _queueName = configuration.LogsQueueName;
            _channel = connection.CreateModel();                           //3 / Definimos el canal de conexion
            _channel.QueueDeclare(_queueName, false, false, false, null);
            _logRepository = LogRepository.GetInstance();
            //_logProcessor = new LogProcessor();
        }

        public void ReceiveServerLogs()
        {
            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);       // 5 - definimos como consumimos los mensajes
            consumer.Received += async (sender, eventArgs) =>
            {
                byte[] body = eventArgs.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                //Log processedLog = _logProcessor.ProcessLog(message);

                //await _logRepository.StoreAsync(processedLog);
                //Console.WriteLine(processedLog.ToString());
            };

            _channel.BasicConsume(_queueName, true, consumer);
        }
    }
}