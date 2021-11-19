using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace NewServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Server is starting...");

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            ServerConfiguration serverConfiguration = new ServerConfiguration()
            {
                RabbitMQServerIP = config.GetSection("ServerConfiguration").GetSection("RabbitMQServerIP").Value,
                RabbitMQServerPort = config.GetSection("ServerConfiguration").GetSection("RabbitMQServerPort").Value,
                LogsQueueName = config.GetSection("ServerConfiguration").GetSection("LogsQueueName").Value,
                ServerPort = config.GetSection("ServerConfiguration").GetSection("ServerPort").Value,
                ServerIP = config.GetSection("ServerConfiguration").GetSection("ServerIP").Value,
                GrpcApiHttpPort = config.GetSection("ServerConfiguration").GetSection("GrpcApiHttpPort").Value,
                GrpcApiHttpsPort = config.GetSection("ServerConfiguration").GetSection("GrpcApiHttpsPort").Value
            };

            await StartUpSocketsServer(serverConfiguration);

            CreateHostBuilder(args, serverConfiguration).Build().Run();

            
        }

        private static async Task StartUpSocketsServer(ServerConfiguration serverConfiguration)
        {
            //LogEmitter logEmitter = new LogEmitter(serverConfiguration);
            await HandleConnections(serverConfiguration);
        }

         private static async Task HandleConnections(ServerConfiguration serverConfiguration)
         {
             ConnectionsHandler connectionsHandler = new ConnectionsHandler(serverConfiguration);
            connectionsHandler.StartListeningAsync();
         }

        
        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        public static IHostBuilder CreateHostBuilder(string[] args, ServerConfiguration serverConfiguration)
        {
            string httpUrl = $"http://{serverConfiguration.ServerIP}:{serverConfiguration.GrpcApiHttpPort}/";
            string httpsUrl = $"https://{serverConfiguration.ServerIP}:{serverConfiguration.GrpcApiHttpsPort}/";

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(httpUrl, httpsUrl);
                });
        }
    }
}