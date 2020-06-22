using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using DidiSoft.OpenSsl;
using DidiSoft.OpenSsl.X509;

namespace GrpcClasses
{

    public static class EncryptionEngine
    {
        public static KeyPair CreateKepPair()
        {
            DidiSoft.OpenSsl.KeyPair kp = DidiSoft.OpenSsl.KeyPair.GenerateKeyPair(KeyAlgorithm.Rsa, KeyLength.Length2048);
            DidiSoft.OpenSsl.PublicKey publicKey = kp.Public;
            DidiSoft.OpenSsl.PrivateKey privateKey = kp.Private;
            return kp;
        }

        public static bool IsCertExist(string CertificateName, string CertificatePath)
        {
            return File.Exists(CertificatePath + CertificateName + ".crt");
        }

        public static Certificate CreateCert(string CertificateName, string CertificatePath, KeyPair kp)
        {
            X509Name certificateProperties = new DidiSoft.OpenSsl.X509.X509Name()
            {
                CommonName = "My CA",
                CountryCode = "US",
                Organization = "SteelCloud",
                OrganizationUnit = "Dev team",
                Locality = "Ashburn, VA",
                EmailAddress = "afrey@steelcloud.com"
            };

            Certificate cert = Certificate.CreateSelfSignedCertificate(kp.Public, kp.Private, certificateProperties);
            cert.Save(CertificatePath + CertificateName + ".crt", true);
            return cert;
        }

        public static void CreatePfx(string CertificateName, string CertificatePath, string password)
        {
            KeyPair kp = CreateKepPair();
            var cert = CreateCert(CertificateName, CertificatePath, kp);
            var pfxStore = new PfxStore();
            pfxStore.AddCertificate(cert.Subject.CommonName, cert);
            pfxStore.AddPrivateKey(cert.Subject.CommonName, kp.Private);
            pfxStore.Save(CertificatePath + CertificateName + ".pfx", password);
        }
    }

}