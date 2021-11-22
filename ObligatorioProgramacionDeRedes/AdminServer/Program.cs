using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Admin server is starting...");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            string httpUrl = $"http://{ConfigurationManager.AppSettings["AdminServerIP"]}:{ConfigurationManager.AppSettings["AdminServerHttpPort"]}/";
            string httpsUrl = $"https://{ConfigurationManager.AppSettings["AdminServerIP"]}:{ConfigurationManager.AppSettings["AdminServerHttpsPort"]}/";

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(httpUrl, httpsUrl);
                });
        }
    }
}
