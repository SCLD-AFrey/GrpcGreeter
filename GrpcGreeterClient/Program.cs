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
            int NumberOfChecks = Int32.Parse(Console.ReadLine());

            var channelCredentials = new SslCredentials(File.ReadAllText(@"C:\Users\AFrey\Documents\Development\SCLD-AFrey\GrpcGreeter\GrpcClasses\Certs\server.pem"));
            
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var endpointClient = new Checker.CheckerClient(channel);

            if (HasPulse(endpointClient).Result)
            {
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

            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
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
    }
}
