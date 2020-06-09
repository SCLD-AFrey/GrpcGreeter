using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using GrpcClasses;


namespace GrpcGreeter
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
                Endpoint = item,
                CheckDateTime = DateTime.Now,
                Result = PlatormList.Contains(item.Platform.ToLower()),
                Message = string.Empty
            };

            if (!PlatormList.Contains(item.Platform.ToLower()))
            {
                itemCheck.Result = false;
                itemCheck.Message += "[Platform is not windows or linux]";
            }

            _logger.LogInformation(itemCheck.Endpoint.IpAddress + ": " + itemCheck.Result.ToString());

            return Task.FromResult(new EndpointCheckReply
            {
                Content = JsonSerializer.Serialize(itemCheck)
            });
        }
    }
}
