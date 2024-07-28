using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Diagnostics;

namespace CertificateGenerator;

public static class Methods
{
    public static void GenerateCertificate(string domainName, string password, string pathSave)
    {
        // Define the certificate properties
        var distinguishedName = new X500DistinguishedName($"CN={domainName}");

        using (RSA rsa = RSA.Create(2048))
        {
            // Create a certificate request
            var certRequest = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Add certificate extensions
            certRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            certRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));
            certRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certRequest.PublicKey, false));

            // Create the self-signed certificate
            var certificate = certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));

            try
            {
                var certificateName = RemoveInvalidFileNameChars(domainName);
                var path = RemoveInvalidPathChars(pathSave);
                CreateFolder(path);
                var outputFileName = Path.Combine(path, certificateName);

                // Export the certificate to a .pfx file
                byte[] pfxBytes = certificate.Export(X509ContentType.Pfx, password);
                var pfxFile = $"{outputFileName}.pfx";
                DeleteFile(pfxFile);
                File.WriteAllBytes(pfxFile, pfxBytes);

                // Export the certificate to a .cer file (DER encoded)
                byte[] cerBytes = certificate.Export(X509ContentType.Cert);
                var cerFile = $"{outputFileName}.cer";
                DeleteFile(cerFile);
                File.WriteAllBytes(cerFile, cerBytes);

                // Export the certificate to a .crt file (Base64 encoded)
                string crtContent = $"-----BEGIN CERTIFICATE-----\n{Convert.ToBase64String(cerBytes, Base64FormattingOptions.InsertLineBreaks)}\n-----END CERTIFICATE-----";
                var crtFile = $"{outputFileName}.crt";
                DeleteFile(crtFile);
                File.WriteAllText(crtFile, crtContent);

                Console.WriteLine($"Self-signed certificates created.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

    private static string RemoveInvalidFileNameChars(string text, char replace = '_')
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            text = text.Replace(c, replace);
        return text;
    }

    private static string RemoveInvalidPathChars(string text, char replace = '_')
    {
        foreach (char c in Path.GetInvalidPathChars())
            text = text.Replace(c, replace);
        return text;
    }

    private static void CreateFolder(string path)
    {
        try
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{ex}");
        }
    }

    private static void DeleteFile(string pathFile)
    {
        try
        {
            if (File.Exists(pathFile))
                File.Delete(pathFile);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{ex}");
        }
    }
}
