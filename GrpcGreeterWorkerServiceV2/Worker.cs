using System;
using System.Collections.Generic;
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
        private readonly string CertName = "GreeterCert";
        private readonly string CertPath = "C:\\certs\\";
        private readonly IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        private readonly int insecPort = 5000;
        private readonly int sslPort = 50051;
        private readonly string password = "pass";


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            if (!EncryptionEngine.IsCertExist(CertName, CertPath))
            {
                EncryptionEngine.CreatePfx(CertName, CertPath, password);
            }

            await Host.CreateDefaultBuilder()
                .UseConsoleLifetime()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseKestrel(serverOptions =>
                        {
                        serverOptions.Listen(ipAddress, sslPort,
                            listenOptions =>
                            {
                                listenOptions
                                    .UseHttps(CertPath + CertName + ".pfx", password)
                                    .Protocols = HttpProtocols.Http2;
                            });
                        serverOptions.Listen(ipAddress, insecPort);
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