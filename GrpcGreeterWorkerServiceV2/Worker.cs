using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using GrpcClasses;

namespace GrpcGreeterWorkerServiceV2
{
    public class Worker : BackgroundService
    {

        private readonly ILogger<Worker> _logger;


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var engine = new EncryptionEngine();

            await Host.CreateDefaultBuilder()
                .UseConsoleLifetime()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseKestrel(serverOptions =>
                        {
                        serverOptions.Listen(IPAddress.Parse("127.0.0.1"), engine.SslPort,
                            listenOptions =>
                            {
                                listenOptions
                                    .UseHttps(engine.CertPath + engine.CertName + ".pfx", engine.CertPassword)
                                    .Protocols = HttpProtocols.Http2;
                            });
                        })
                        .UseStartup<GrpcServerStartup>();
                })
                .Build()
                .StartAsync(stoppingToken);
        }

        public class GrpcServerStartup
        {
            public void ConfigureServices(IServiceCollection p_services)
            {
                p_services.AddGrpc();
                p_services.AddSingleton<Services.CheckerService>();
            }

            public void Configure(IApplicationBuilder p_app)
            {
                p_app.UseRouting();

                p_app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGrpcService<Services.CheckerService>();
                });
            }
        }
    }
}