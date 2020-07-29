using System;
using System.Threading.Tasks;
using GrpcGreeter;
using Grpc.Net.Client;
using System.Text.Json;
using System.Collections.Generic;
using GrpcClasses;
using Grpc.Core;
using System.IO;
using System.Diagnostics;
using System.Management.Automation;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using DidiSoft.OpenSsl;
using DidiSoft.OpenSsl.X509;

using System.Collections.ObjectModel;
using System.Dynamic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.VisualBasic;

namespace GrpcGreeterClientSsl
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Utilities utils = new Utilities();
            var engine = new EncryptionEngine();

            var script = "Test-NetConnection -ComputerName 127.0.0.1 -Port " + engine.SslPort;

            Console.WriteLine("gRPC Greet Client - SSL");

            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(script);

            pipeline.Commands.Add("Out-String");

            Collection<PSObject> results = pipeline.Invoke();

            runspace.Close();

            dynamic o = new ExpandoObject();

            foreach (PSObject obj in results)
            {
                //Console.Write(obj.ToString());

                string[] stringSeparators = new string[] { "\r\n" };
                string[] lines = obj.ToString().Split(stringSeparators, StringSplitOptions.None);
                Console.WriteLine("Nr. Of items in list: " + lines.Length);
                foreach (string s in lines)
                {
                    Console.WriteLine(s);
                }

            }



                /*
            using (PowerShell ps = PowerShell.Create())
            {




                ps.AddScript(script);



                var pipelineObjects = await ps.InvokeAsync().ConfigureAwait(false);
                foreach (var item in pipelineObjects)
                {
                    Console.WriteLine(item.Properties.ToString());
                }
            }
                */



            Console.WriteLine("Enter number of checks:");
            var numberOfChecks = Int32.Parse(Console.ReadLine());

            var cert = Certificate.Load(engine.CertPath + engine.CertName + ".crt");
            var cert509 = cert.ToX509Certificate2();

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert509);

            var httpClient = new HttpClient(handler);

            using var channel = GrpcChannel.ForAddress("https://127.0.0.1:" + engine.SslPort.ToString(), new GrpcChannelOptions
                {
                    HttpClient = httpClient
                });
            var endpointClient = new Checker.CheckerClient(channel);

            var stopwatch = new Stopwatch();
            List<EndpointItem> endpointItemList = utils.CreateEndpointList(numberOfChecks);

            Console.WriteLine(endpointItemList.Count.ToString() + " items to process");

            stopwatch.Start();

            foreach (var item in endpointItemList)
            {
                var request = new EndpointCheckRequest()
                {
                    Content = JsonSerializer.Serialize(item)
                };
                var check = new EndpointItemCheck();
                try
                {
                    var reply = await endpointClient.CheckEndpointAsync(request);
                    check = JsonSerializer.Deserialize<EndpointItemCheck>(reply.Content);
                }
                catch (Exception e)
                {
                    check = new EndpointItemCheck()
                    {
                        Endpoint = item,
                        Message = e.Message
                    };
                }
                Console.WriteLine(check.Endpoint.Name + " - " + check.Result + " " + check.Message);
            }

            stopwatch.Stop();
            Console.WriteLine("Finshed " + numberOfChecks + " records in " + stopwatch.Elapsed + " seconds");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

    }
}