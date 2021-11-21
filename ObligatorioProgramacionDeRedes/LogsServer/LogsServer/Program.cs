using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogsServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Logs server is starting...");
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            LogServerConfiguration logsServerConfiguration = new LogServerConfiguration()
            {
                RabbitMQServerIP = config.GetSection("LogsServerConfiguration").GetSection("RabbitMQServerIP").Value,
                RabbitMQServerPort = config.GetSection("LogsServerConfiguration").GetSection("RabbitMQServerPort").Value,
                LogsQueueName = config.GetSection("LogsServerConfiguration").GetSection("LogsQueueName").Value,
                WebApiHttpPort = config.GetSection("LogsServerConfiguration").GetSection("WebApiHttpPort").Value,
                WebApiHttpsPort = config.GetSection("LogsServerConfiguration").GetSection("WebApiHttpsPort").Value,
            };
            
            SetupLogListener(logsServerConfiguration);
            CreateHostBuilder(args, logsServerConfiguration).Build().Run();
        }
        
        public static void SetupLogListener(LogServerConfiguration configuration)
        {
            LogReceiver logReceiver = new LogReceiver(configuration);
            logReceiver.ReceiveServerLogs();
        }
        public static IHostBuilder CreateHostBuilder(string[] args, LogServerConfiguration configuration)
        {
            string httpUrl = $"http://localhost:{configuration.WebApiHttpPort}/";
            string httpsUrl = $"https://localhost:{configuration.WebApiHttpsPort}/";

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(httpUrl, httpsUrl);
                });
        }
    }
}