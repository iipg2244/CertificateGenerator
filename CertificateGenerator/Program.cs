using CertificateGenerator;
using Spectre.Console;

string domainName = string.Empty;
string password = string.Empty;
string pathSave = string.Empty;

if (args.Length == 3)
{
    Console.WriteLine("Usage: CertificateGenerator <domainName> <password> <pathSave>");
    domainName = args[0];
    password = args[1];
    pathSave = args[2];
}
else
{
    domainName = AnsiConsole.Prompt(new TextPrompt<string>("Enter the domain name:")
        .AllowEmpty()
        .Validate(x => !string.IsNullOrWhiteSpace(x) ? ValidationResult.Success() : ValidationResult.Error("[red]Domain name cannot be empty[/]")));
    password = AnsiConsole.Prompt(new TextPrompt<string>("Enter the password:")
        .AllowEmpty()
        .Secret()
        .Validate(x => !string.IsNullOrWhiteSpace(x) ? ValidationResult.Success() : ValidationResult.Error("[red]Password cannot be empty[/]")));
    string decision = AnsiConsole.Prompt(new TextPrompt<string>("Do you want to save the generated certificates in the application folder? [[y/n]] (y):")
        .AddChoice("y")
        .AddChoice("n")
        .AllowEmpty()
        .DefaultValue("y"));
    if (decision.Equals("y"))
    {
        pathSave = Methods.CreateFolderInProgramPath(domainName);
    }
    else
    {
        pathSave = AnsiConsole.Prompt(new TextPrompt<string>("Enter the path to save the generated certificates:")
            .AllowEmpty()
            .Validate(x => !string.IsNullOrWhiteSpace(x) ? ValidationResult.Success() : ValidationResult.Error("[red]Path cannot be empty[/]")));
    }
}

try
{
    Methods.GenerateCertificate(domainName, password, pathSave);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
