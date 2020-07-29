using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using DidiSoft.OpenSsl;
using DidiSoft.OpenSsl.X509;

namespace GrpcClasses
{

    public class EncryptionEngine
    {
        public string CertPath = "C:\\certs\\";
        public string CertName = "GrpcGreeterCert";
        public string CertPassword = "P@ssword";
        public int SslPort = 50051;

        public KeyPair CreateKepPair(KeyLength p_keyLength = KeyLength.Length1024)
        {
            DidiSoft.OpenSsl.KeyPair kp = DidiSoft.OpenSsl.KeyPair.GenerateKeyPair(KeyAlgorithm.Rsa, p_keyLength);
            return kp;
        }

        public void CreateCert(string p_certificateName, string p_certificatePath, KeyPair p_keyPair)
        {
            Certificate cert = Certificate.CreateSelfSignedCertificate(p_keyPair.Public, p_keyPair.Private, CertificateX509Name(p_certificateName));
            cert.Save(p_certificatePath + p_certificateName + ".crt", true);
            //var cert2 = cert.ToX509Certificate();
            //File.WriteAllBytes(CertPath + CertName + ".crt", cert2.Export(X509ContentType.Cert));
        }

        public void CreatePfx(string p_certificateName, string p_certificatePath, string p_password)
        {
            KeyPair kp = CreateKepPair();
            CreateCert(p_certificateName, p_certificatePath, kp);
            var cert = Certificate.Load(CertPath + CertName + ".crt");
            var pfxStore = new PfxStore();
            pfxStore.AddCertificate(cert.Subject.CommonName, cert);
            pfxStore.AddPrivateKey(cert.Subject.CommonName, kp.Private);
            pfxStore.Save(p_certificatePath + p_certificateName + ".pfx", p_password);
        }
        

        private X509Name CertificateX509Name(string p_certificateName)
        {
            return new DidiSoft.OpenSsl.X509.X509Name()
            {
                CommonName = p_certificateName,
                CountryCode = "US",
                Organization = "SteelCloud",
                OrganizationUnit = "Dev Team",
                Locality = "Ashburn, VA",
                EmailAddress = "afrey@steelcloud.com"
            };
        }

    }

}