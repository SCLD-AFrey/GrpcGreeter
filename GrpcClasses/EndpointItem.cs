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
}