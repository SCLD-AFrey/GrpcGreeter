using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using GrpcClasses;
using GrpcGreeter;


namespace GrpcGreeterWorkerService
{
    public class CheckerService : Checker.CheckerBase
    {
        private readonly ILogger<CheckerService> _logger;
        public CheckerService(ILogger<CheckerService> logger)
        {
            _logger = logger;
        }

        public override Task<EndpointCheckReply> CheckEndpoint(EndpointCheckRequest request, ServerCallContext context)
        {
            List<string> PlatormList = new List<string>(){"windows","linux"};
            var item = JsonSerializer.Deserialize<EndpointItem>(request.Content);
            var itemCheck = new EndpointItemCheck()
            {
                Endpoint = item
            };

            if (!PlatormList.Contains(item.Platform.ToLower()))
            {
                itemCheck.Message += "Platform is not windows or linux";
            }

            var reply = new EndpointCheckReply
            {
                Content = JsonSerializer.Serialize(itemCheck)
            };

            _logger.LogInformation(JsonSerializer.Serialize(reply));

            return Task.FromResult(reply);
        }

        public override Task<HeartbeatCheckReply> HeartbeatCheck(HeartbeatCheckRequest request,
            ServerCallContext context)
        {
            return Task.FromResult(new HeartbeatCheckReply()
            {
                Reply = true
            });
        }
    }
}
