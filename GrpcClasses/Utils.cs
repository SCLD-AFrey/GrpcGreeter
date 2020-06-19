using System;
using System.Collections.Generic;
using System.Net;

namespace GrpcClasses
{

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
                    Platform = PlatformList[rand.Next(PlatformList.Count)]
                });
            }

            return EndpointItemList;
        }
    }

}