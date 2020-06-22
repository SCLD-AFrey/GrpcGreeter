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
using Microsoft.Extensions.Configuration;
using GrpcClasses;

namespace GrpcGreeterWorkerServiceV2
{
    public class Worker : BackgroundService
    {

        private readonly ILogger<Worker> _logger;
        private readonly string CertName = "GreeterCert";
        private readonly string CertPath = "C:\\certs\\";
        private IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        private int insecPort = 5000;
        private int sslPort = 50051;
        private string password = "pass";


        public Worker(ILogger<Worker> logger, IConfiguration configuration)
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
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddGrpc();
                services.AddSingleton<Services.CheckerService>();
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGrpcService<Services.CheckerService>();
                });
            }
        }
    }
}