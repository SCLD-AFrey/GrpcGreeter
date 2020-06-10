using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Core.Utils;

namespace GrpcClasses
{

    public static class Creds
    {
        private const string CertLoc = @"C:\Users\AFrey\Documents\Development\SCLD-AFrey\GrpcGreeter\GrpcClasses\Certs";

        public static string ClientCertAuthorityPath => CertLoc + @"\client.pem";
        public static string ServerCertChainPath =>CertLoc + @"\server.pem";
        public static string ServerPrivateKeyPath => CertLoc + @"\privateKey.key";

        public static SslCredentials CreateSslCredentials() => new SslCredentials(File.ReadAllText(ClientCertAuthorityPath));

        public static SslServerCredentials CreateSslServerCredentials()
        {
            var keyCertPair = new KeyCertificatePair(
                File.ReadAllText(ServerCertChainPath),
                File.ReadAllText(ServerPrivateKeyPath));
            return new SslServerCredentials(new[] { keyCertPair });
        }
        private static string GetPath(string relativePath)
        {
            var assemblyDir = Path.GetDirectoryName(typeof(Creds).GetTypeInfo().Assembly.Location);
            return Path.Combine(assemblyDir, relativePath);
        }
    }


}