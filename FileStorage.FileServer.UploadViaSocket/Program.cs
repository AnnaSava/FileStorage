// See https://aka.ms/new-console-template for more information
using Athn.Helpers.MimeType;
using FileStorage;
using FileStorage.Data.MongoDb;
using FileStorage.FileServer.UploadViaSocket;
using FileStorage.Helpers;
using FileStorage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

Console.WriteLine("Hello, World!");

IConfigurationRoot config = GetConfiguration();
var services = new ServiceCollection();

services.Configure<DatabaseSettings>(
   config.GetSection(nameof(DatabaseSettings)));

services.AddSingleton<IDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

services.AddSingleton<HashHelper>();
services.AddSingleton<MimeTypeChecker>();
services.AddSingleton<IFileRepository, FileRepository>();
services.AddSingleton<FileProcessingService>();

ServiceProvider serviceProvider = services.BuildServiceProvider();

await new FTServerCode(serviceProvider.GetService<FileProcessingService>()).StartServer();

IConfigurationRoot GetConfiguration()
{
    string appsettingsPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "appsettings.json"));

    return new ConfigurationBuilder()
        .AddJsonFile(appsettingsPath, true, true)
        .Build();
}