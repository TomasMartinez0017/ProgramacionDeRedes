using System;
using System.Collections.Generic;
using System.Configuration;
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
            
            SetupLogListener();
            CreateHostBuilder(args).Build().Run();
        }
        
        public static void SetupLogListener()
        {
            LogReceiver logReceiver = new LogReceiver();
            logReceiver.ReceiveServerLogs();
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            string httpUrl = $"http://localhost:{ConfigurationManager.AppSettings["WebApiHttpPort"]}/";
            string httpsUrl = $"https://localhost:{ConfigurationManager.AppSettings["WebApiHttpsPort"]}/";

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(httpUrl, httpsUrl);
                });
        }
    }
}