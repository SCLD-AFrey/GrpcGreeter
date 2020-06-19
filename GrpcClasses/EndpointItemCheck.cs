using System;
using System.Collections.Generic;
using System.Net;

namespace GrpcClasses
{
    public class EndpointItemCheck
    {
        public EndpointItem Endpoint { get; set; } = new EndpointItem();
        public DateTime CheckDateTime { get; set; } = DateTime.Now;
        public bool Result { get; set; } = false;
        public string Message { get; set; } = String.Empty;
    }

}