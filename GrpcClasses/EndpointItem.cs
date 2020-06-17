using System;
using System.Collections.Generic;
using System.Net;

namespace GrpcClasses
{

    public class EndpointItem
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string Platform { get; set; }
        public int BatchID { get; set; } = 0;
    }

    public class EndpointItemCheck
    {
        public EndpointItem Endpoint { get; set; } = new EndpointItem();
        public DateTime CheckDateTime { get; set; } = DateTime.Now;
        public bool Result { get; set; } = false;
        public string Message { get; set; } = String.Empty;
    }

    public static class CommonVars
    {
        public static IPAddress IpAddress { get; } = IPAddress.Parse("127.0.0.1");
        public static int Port = 5001;

        public static string LogSource = "GrpcLogSource";
        public static string LogName = "GrpcGreeterLog";
    }

    public static class Utils
    {
        public static List<EndpointItem> CreateEndpointList(int count)
        {
            List<EndpointItem> EndpointItemList = new List<EndpointItem>();
            List<string> PlatformList = new List<string>() { "windows", "linux", "other" };

            for (int i = 0; i < count; i++)
            {
                Random rand = new Random();
                EndpointItemList.Add(new EndpointItem()
                {
                    Name = "Test " + i.ToString(),
                    IpAddress = string.Format("{0}.{1}.{2}.{3}", rand.Next(1, 256), rand.Next(1, 256), rand.Next(1, 256), rand.Next(1, 256)),
                    Platform = PlatformList[rand.Next(PlatformList.Count)],
                    BatchID = i + 1
                });
            }

            return EndpointItemList;
        }
    }

}