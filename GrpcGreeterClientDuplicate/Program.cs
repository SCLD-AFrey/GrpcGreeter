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

namespace GrpcGreeterClientDuplicate
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("gRPC Greet Client - Duplicate");
            Console.WriteLine("Enter number of checks:");
            int NumberOfChecks = Int32.Parse(Console.ReadLine());

            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Checker.CheckerClient(channel);
            
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
                    var _reply = await client.CheckEndpointAsync(_request);
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
