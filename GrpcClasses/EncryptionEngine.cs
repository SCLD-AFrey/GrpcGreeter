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
            return kp;
        }

        public static bool IsCertExist(string p_certificateName, string p_certificatePath)
        {
            return File.Exists(p_certificatePath + p_certificateName + ".crt");
        }

        public static Certificate CreateCert(string p_certificateName, string p_certificatePath, KeyPair p_keyPair)
        {
            X509Name certificateProperties = new DidiSoft.OpenSsl.X509.X509Name()
            {
                CommonName = p_certificateName,
                CountryCode = "US",
                Organization = "SteelCloud",
                OrganizationUnit = "Dev team",
                Locality = "Ashburn, VA",
                EmailAddress = "afrey@steelcloud.com"
            };

            Certificate cert = Certificate.CreateSelfSignedCertificate(p_keyPair.Public, p_keyPair.Private, certificateProperties);
            cert.Save(p_certificatePath + p_certificateName + ".crt", true);
            return cert;
        }

        public static void CreatePfx(string p_certificateName, string p_certificatePath, string p_password)
        {
            KeyPair kp = CreateKepPair();
            var cert = CreateCert(p_certificateName, p_certificatePath, kp);
            var pfxStore = new PfxStore();
            pfxStore.AddCertificate(cert.Subject.CommonName, cert);
            pfxStore.AddPrivateKey(cert.Subject.CommonName, kp.Private);
            pfxStore.Save(p_certificatePath + p_certificateName + ".pfx", p_password);
        }

    }

}