using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewServer.Managers;
using NewServer.Services;

namespace NewServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            RegisterServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<UserService>();
                endpoints.MapGrpcService<GameService>();

                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
            });
        }

        private void RegisterServices(IServiceCollection services)
        {
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

            services.AddScoped<ServerConfiguration>(s => serverConfiguration);
            services.AddScoped<UserManager, UserManager>();

            //services.AddScoped<LogEmitter, LogEmitter>();
            //services.AddScoped<BaseMapper, BaseMapper>();
            //services.AddScoped<PostMapper, PostMapper>();
            //services.AddScoped<ThemeMapper, ThemeMapper>();
            //services.AddScoped<IThemeManager, ThemeManager>();
            //services.AddScoped<IDeserializer, Deserializer>();
            //services.AddScoped<IPostManager, PostManager>();
            //services.AddScoped<IUserRepository>(u => UserRepository.GetInstance());
        }
    }
}