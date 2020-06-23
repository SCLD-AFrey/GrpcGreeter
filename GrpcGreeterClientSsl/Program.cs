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
            var numberOfChecks = Int32.Parse(Console.ReadLine());

            //var cert = X509Certificate.CreateFromCertFile("C:\\certs\\GreeterCert.crt");
            X509Certificate cert = X509Certificate.CreateFromSignedFile("C:\\certs\\GreeterCert.crt");

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);
            var httpClient = new HttpClient(handler);

            using var channel = GrpcChannel.ForAddress("https://localhost:50051", new GrpcChannelOptions
                {
                    HttpClient = httpClient
            })
                ;
            var endpointClient = new Checker.CheckerClient(channel);

            var stopwatch = new Stopwatch();
            List<EndpointItem> endpointItemList = Utils.CreateEndpointList(numberOfChecks);

            Console.WriteLine(endpointItemList.Count.ToString() + " items to process");

            stopwatch.Start();

            foreach (var item in endpointItemList)
            {
                var request = new EndpointCheckRequest()
                {
                    Content = JsonSerializer.Serialize(item)
                };
                var check = new EndpointItemCheck();
                try
                {
                    var reply = await endpointClient.CheckEndpointAsync(request);
                    check = JsonSerializer.Deserialize<EndpointItemCheck>(reply.Content);
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

            stopwatch.Stop();
            Console.WriteLine("Finshed " + numberOfChecks + " records in " + stopwatch.Elapsed + " seconds");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

    }
}