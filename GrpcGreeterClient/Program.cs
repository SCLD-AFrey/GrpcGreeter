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

            //if (HasPulse(endpointClient).Result)
            //{
                var stopwatch = new Stopwatch();
                List<EndpointItem> EndpointItemList = Utils.CreateEndpointList(numberOfChecks);

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

            //}
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        /*
        private static async Task<bool> HasPulse(Checker.CheckerClient client)
        {
            bool _pulse = false;
            try
            {
                var Heartbeat = await client.HeartbeatCheckAsync(new HeartbeatCheckRequest());
                _pulse = Heartbeat.Reply;
            }
            catch (Exception e)
            {
                _pulse = false;
            }

            if (_pulse)
            {
                Console.WriteLine("Server is alive");
            }
            else
            {
                Console.WriteLine("Server is not responding");
            }

            return _pulse;

        }
        */
    }
}
