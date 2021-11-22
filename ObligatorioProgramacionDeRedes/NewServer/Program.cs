using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogsHelper;
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

            await StartUpSocketsServer();

            CreateHostBuilder(args).Build().Run();
        }

        private static async Task StartUpSocketsServer()
        {
            LogEmitter logEmitter = new LogEmitter();
            await HandleConnections();
            
        }

         private static async Task HandleConnections()
         {
            ConnectionsHandler connectionsHandler = new ConnectionsHandler();
            connectionsHandler.StartListeningAsync();
         }

        
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            string httpUrl = $"http://{ConfigurationManager.AppSettings["ServerIP"]}:{ConfigurationManager.AppSettings["GrpcApiHttpPort"]}/";
            string httpsUrl = $"https://{ConfigurationManager.AppSettings["ServerIP"]}:{ConfigurationManager.AppSettings["GrpcApiHttpsPort"]}/";

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(httpUrl, httpsUrl);
                });
        }
    }
}