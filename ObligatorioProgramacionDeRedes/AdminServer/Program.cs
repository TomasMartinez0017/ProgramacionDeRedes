using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Admin server is starting...");

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            AdminServerConfiguration adminServerConfiguration = new AdminServerConfiguration()
            {
                AdminServerIP = config.GetSection("AdminServerConfiguration").GetSection("AdminServerIP").Value,
                AdminServerHttpPort = config.GetSection("AdminServerConfiguration").GetSection("AdminServerHttpPort").Value,
                AdminServerHttpsPort = config.GetSection("AdminServerConfiguration").GetSection("AdminServerHttpsPort").Value,
                GrpcServerApiHttpPort = config.GetSection("AdminServerConfiguration").GetSection("GrpcServerApiHttpPort").Value,
                GrpcServerApiHttpsPort = config.GetSection("AdminServerConfiguration").GetSection("GrpcServerApiHttpsPort").Value,
                GrpcServerIP = config.GetSection("AdminServerConfiguration").GetSection("GrpcServerIP").Value
            };
            CreateHostBuilder(args, adminServerConfiguration).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, AdminServerConfiguration configuration)
        {
            string httpUrl = $"http://{configuration.AdminServerIP}:{configuration.AdminServerHttpPort}/";
            string httpsUrl = $"https://{configuration.AdminServerIP}:{configuration.AdminServerHttpsPort}/";

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(httpUrl, httpsUrl);
                });
        }
    }
}
