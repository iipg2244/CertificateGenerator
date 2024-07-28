using CertificateGenerator;

if (args.Length < 3)
{
    Console.WriteLine("Usage: CertificateGenerator <domainName> <password> <pathSave>");
    return;
}

string domainName = args[0];
string password = args[1];
string pathSave = args[2];

try
{
    Methods.GenerateCertificate(domainName, password, pathSave);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}