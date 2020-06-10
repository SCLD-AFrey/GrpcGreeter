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
        public EndpointItem Endpoint { get; set; } = new EndpointItem();
        public DateTime CheckDateTime { get; set; } = DateTime.Now;
        public bool Result { get; set; } = false;
        public string Message { get; set; } = String.Empty;
    }



}