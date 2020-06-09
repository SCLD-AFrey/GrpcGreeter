using System;

namespace GrpcClasses
{

    public class EndpointItem
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string Platform { get; set; }
    }

    public class EndpointItemCheck
    {
        public EndpointItem Endpoint { get; set; }
        public DateTime CheckDateTime { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; }
    }
}