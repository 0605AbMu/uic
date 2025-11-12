// See https://aka.ms/new-console-template for more information

using System.Text;
using UIC;

const string tempPath = "temp";
string basePath = Path.Join(AppContext.BaseDirectory, tempPath);
const string crtName = "test.pem";

Console.WriteLine("Process started {0}.", DateTime.Now);

Init();

var identity1 = new UserIdentity()
{
    UserId = 42,
    Username = "AsyncPro",
    Email = "dev@example.com",
    Permissions = ["read", "write"]
};

var derBuffer = DerService.EncodeUserIdentity(identity1);
var pemResult = DerService.ToPem(derBuffer);

Console.WriteLine("Pem Result:\n\n{0}\n", pemResult);

var resultFilePath = SaveToTemp(crtName, Encoding.UTF8.GetBytes(pemResult));
Console.WriteLine("Encoding finished: {0}", resultFilePath);

var parsedUser = DerService.DecodeUserIdentity(DerService.FromPem(ReadFromTemp(crtName)));
Console.WriteLine("Parsed user: {0}", parsedUser);

Console.WriteLine("Process finished {0}.", DateTime.Now);

return;

void Init()
{
    if (!Directory.Exists(basePath))
        Directory.CreateDirectory(basePath);
}

#region Temp Storage Support

string SaveToTemp(string name, byte[] buffer)
{
    var filePath = Path.Join(basePath, name);
    using var fileStream = File.OpenWrite(filePath);
    fileStream.Write(buffer);

    return filePath; //save filed path as result
}

string ReadFromTemp(string name)
{
    var filePath = Path.Join(basePath, name);
    using var fileStream = File.OpenRead(filePath);
    using StreamReader reader = new StreamReader(fileStream);

    return reader.ReadToEnd();
}

#endregion