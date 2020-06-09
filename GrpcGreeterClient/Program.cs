using System;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using DevExpress.Xpo;
using GrpcGreeter;
using Grpc.Net.Client;
using System.Text.Json;
using System.Text.Json.Serialization;
using DevExpress.Xpo.Helpers;
using System.Collections.Generic;
using GrpcClasses;

namespace GrpcGreeterClient
{
    class Program
    {

        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var endpointClient = new Checker.CheckerClient(channel);

            List<EndpointItem> EndpointItemList = new List<EndpointItem>();
            List<string> PlatformList = new List<string>() { "windows", "linux", "other" };

            for (int i = 0; i < 100; i++)
            {
                Random rand = new Random();
                EndpointItemList.Add(new EndpointItem()
                {
                    Name = "Test " + i.ToString(),
                    IpAddress = string.Format("{0}.{1}.{2}.{3}", rand.Next(1, 256), rand.Next(1, 256), rand.Next(1, 256), rand.Next(1, 256)),
                    Platform = PlatformList[rand.Next(PlatformList.Count)]
                });
            }

            Console.WriteLine(EndpointItemList.Count.ToString() + " items to process");

            foreach (var item in EndpointItemList)
            {
                var _request = new EndpointCheckRequest()
                {
                    Content = JsonSerializer.Serialize(item)
                };
                var _reply = await endpointClient.CheckEndpointAsync(_request);
                var check = JsonSerializer.Deserialize<EndpointItemCheck>(_reply.Content);
                Console.WriteLine(check.Endpoint.Name + " - " + check.Result + " " + check.Message);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
