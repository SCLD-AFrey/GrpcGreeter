using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using GrpcClasses;
using GrpcGreeter;

namespace GrpcGreeterWorkerServiceV2.Services
{
    public class CheckerService : Checker.CheckerBase
    {
        private readonly ILogger<CheckerService> _logger;
        public CheckerService(ILogger<CheckerService> logger)
        {
            _logger = logger;
        }

        public override Task<EndpointCheckReply> CheckEndpoint(EndpointCheckRequest p_request, ServerCallContext p_context)
        {
            List<string> platformList = new List<string>() { "windows", "linux" };
            var item = JsonSerializer.Deserialize<EndpointItem>(p_request.Content);
            var itemCheck = new EndpointItemCheck()
            {
                Endpoint = item
            };

            if (!platformList.Contains(item.Platform.ToLower()))
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

        public override Task<HeartbeatCheckReply> HeartbeatCheck(HeartbeatCheckRequest p_request, ServerCallContext p_context)
        {
            return Task.FromResult(new HeartbeatCheckReply()
            {
                Reply = true
            });
        }
    }

}