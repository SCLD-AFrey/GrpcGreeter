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

namespace GrpcGreeterClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("gRPC Greet Client - Sync");
            Console.WriteLine("Enter number of checks:");
            var numberOfChecks = Int32.Parse(Console.ReadLine());

            using var channel = GrpcChannel.ForAddress("https://localhost:5000");
            var endpointClient = new Checker.CheckerClient(channel);


                var stopwatch = new Stopwatch();
                var utils = new Utilities();
                List<EndpointItem> EndpointItemList = utils.CreateEndpointList(numberOfChecks);

                Console.WriteLine(EndpointItemList.Count.ToString() + " items to process");

                stopwatch.Start();

                foreach (var item in EndpointItemList)
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
