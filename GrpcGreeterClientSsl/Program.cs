using System;
using System.Threading.Tasks;
using GrpcGreeter;
using Grpc.Net.Client;
using System.Text.Json;
using System.Collections.Generic;
using GrpcClasses;
using Grpc.Core;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using DidiSoft.OpenSsl;

namespace GrpcGreeterClientSsl
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("gRPC Greet Client - SSL");
            Console.WriteLine("Enter number of checks:");
            int NumberOfChecks = Int32.Parse(Console.ReadLine());

            //var cert = X509Certificate.CreateFromCertFile("C:\\certs\\GreeterCert.crt");
            X509Certificate cert = X509Certificate2.CreateFromSignedFile("C:\\certs\\GreeterCert.crt");

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);
            var httpClient = new HttpClient(handler);

            using var channel = GrpcChannel.ForAddress("https://localhost:50051", new GrpcChannelOptions
                {
                    HttpClient = httpClient
            })
                ;
            var endpointClient = new Checker.CheckerClient(channel);

            var _stopwatch = new Stopwatch();
            List<EndpointItem> EndpointItemList = Utils.CreateEndpointList(NumberOfChecks);

            Console.WriteLine(EndpointItemList.Count.ToString() + " items to process");

            _stopwatch.Start();

            foreach (var item in EndpointItemList)
            {
                var _request = new EndpointCheckRequest()
                {
                    Content = JsonSerializer.Serialize(item)
                };
                var check = new EndpointItemCheck();
                try
                {
                    var _reply = await endpointClient.CheckEndpointAsync(_request);
                    check = JsonSerializer.Deserialize<EndpointItemCheck>(_reply.Content);
                }
                catch (Exception e)
                {
                    check = new EndpointItemCheck()
                    {
                        Endpoint = item,
                        Message = e.Message
                    };
                }
                Console.WriteLine(check.Endpoint.Name + " - " + check.Result + " " + check.Message);
            }

            _stopwatch.Stop();
            Console.WriteLine("Finshed " + NumberOfChecks + " records in " + _stopwatch.Elapsed + " seconds");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

    }
}