using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using GrpcClasses;
using System.Diagnostics;


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


            if (!EventLog.SourceExists(CommonVars.LogSource))
            {
                EventLog.CreateEventSource(CommonVars.LogSource, CommonVars.LogName);
            }
            EventLog myLog = new EventLog();
            myLog.Source = CommonVars.LogSource;

            var reply = new EndpointCheckReply();
            var itemCheck = new EndpointItemCheck
            {
                Endpoint = JsonSerializer.Deserialize<EndpointItem>(request.Content)
            };

            try
            {
                List<string> PlatormList = new List<string>() { "windows", "linux" };

                if (!PlatormList.Contains(itemCheck.Endpoint.Platform.ToLower()))
                {
                    itemCheck.Message += "Platform is not windows or linux";
                }

                if (itemCheck.Endpoint.BatchID == 6)
                {
                    throw new Exception("THIS IS IN ERROR");
                }

                myLog.WriteEntry(
                    string.Format("{0}-[{1}] Succeeded"
                        ,itemCheck.Endpoint.Name
                        ,itemCheck.Endpoint.IpAddress)
                    , (itemCheck.Result) ? EventLogEntryType.Information : EventLogEntryType.Warning
                    , itemCheck.Endpoint.BatchID);
            }
            catch (Exception e)
            {
                itemCheck.Message = string.Format("{0}-[{1}] Failed - see Event Log for details", itemCheck.Endpoint.Name, itemCheck.Endpoint.IpAddress);
                myLog.WriteEntry(String.Format("{0}-[{1}]-{2}", itemCheck.Endpoint.Name, itemCheck.Endpoint.IpAddress, e.Message), EventLogEntryType.Error, itemCheck.Endpoint.BatchID);
            } 
            
            reply.Content = JsonSerializer.Serialize(itemCheck);




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
