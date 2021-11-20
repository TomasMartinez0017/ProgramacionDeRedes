namespace LogsServer
{
    public class LogServerConfiguration
    {
        public string RabbitMQServerIP { get; set; }
        public string RabbitMQServerPort { get; set; }
        public string LogsQueueName { get; set; }
        public string WebApiHttpPort { get; set; }
        public string WebApiHttpsPort { get; set; }
    }
}