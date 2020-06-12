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

namespace GrpcGreeterClientAsync
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("gRPC Greet Client - Asychronous");
            Console.WriteLine("Enter number of checks:");
            int NumberOfChecks = Int32.Parse(Console.ReadLine());

            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Checker.CheckerClient(channel);
            
            List<EndpointItem> EndpointItemList = Utils.CreateEndpointList(NumberOfChecks);

            Console.WriteLine(EndpointItemList.Count.ToString() + " items to process");

            var _stopwatch = new Stopwatch();
            _stopwatch.Start();

            foreach (var item in EndpointItemList)
            {
                await ProcessItemTaskAsync(item, client);
            }
            _stopwatch.Stop();
            Console.WriteLine("Finshed " + NumberOfChecks + " records in " + _stopwatch.Elapsed + " seconds");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }


        public static async Task<EndpointItemCheck> ProcessItemTaskAsync(EndpointItem item, Checker.CheckerClient client)
        {
            EndpointItemCheck check;
            var _request = new EndpointCheckRequest()
            {
                Content = JsonSerializer.Serialize(item)
            };
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

            return check;
        }
    }
}
