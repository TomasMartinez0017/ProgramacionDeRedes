using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddServices(services);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddServices(IServiceCollection services)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            AdminServerConfiguration configuration = new AdminServerConfiguration()
            {
                AdminServerIP = config.GetSection("AdminServerConfiguration").GetSection("AdminServerIP").Value,
                AdminServerHttpPort = config.GetSection("AdminServerConfiguration").GetSection("AdminServerHttpPort").Value,
                AdminServerHttpsPort = config.GetSection("AdminServerConfiguration").GetSection("AdminServerHttpsPort").Value,
                GrpcServerApiHttpPort = config.GetSection("AdminServerConfiguration").GetSection("GrpcServerApiHttpPort").Value,
                GrpcServerApiHttpsPort = config.GetSection("AdminServerConfiguration").GetSection("GrpcServerApiHttpsPort").Value,
                GrpcServerIP = config.GetSection("AdminServerConfiguration").GetSection("GrpcServerIP").Value
            };
            var channel = GrpcChannel.ForAddress($"https://{configuration.GrpcServerIP}:{configuration.GrpcServerApiHttpsPort}");

            services.AddScoped<UserAdmin.UserAdminClient>(t => new UserAdmin.UserAdminClient(channel));
            services.AddScoped<GameAdmin.GameAdminClient>(t => new GameAdmin.GameAdminClient(channel));
        }
    }
}
