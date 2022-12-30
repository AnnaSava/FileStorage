// See https://aka.ms/new-console-template for more information
using Athn.Helpers.MimeType;
using FileStorage;
using FileStorage.Data.MongoDb;
using FileStorage.FileServer.UploadViaRabbitMQ;
using FileStorage.Helpers;
using FileStorage.Models;
using FileStorage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.Json;

Console.WriteLine("Hello, World!");

IConfigurationRoot config = GetConfiguration();

string WorkerName = Assembly.GetExecutingAssembly().FullName;
string HostName = config.GetSection("RabbitMq:HostName").Value;
const string QueueName = "filestosave";

Console.WriteLine(WorkerName);

IServiceCollection services = new ServiceCollection();
services.Configure<DatabaseSettings>(
   config.GetSection(nameof(DatabaseSettings)));

services.AddSingleton<IDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

services.Configure<FileServerSettings>(
   config.GetSection(nameof(FileServerSettings)));

services.AddSingleton<FileServerSettings>(sp => sp.GetRequiredService<IOptions<FileServerSettings>>().Value);

services.AddSingleton<HashHelper>();
services.AddSingleton<MimeTypeChecker>();
services.AddSingleton<IFileRepository, FileRepository>();
services.AddSingleton<FileProcessingService>();

var work = new Work(HostName, QueueName);
work.Execute(DoAction, services);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

static async void DoAction(string message, IServiceCollection services)
{
    Console.WriteLine(message);
    ServiceProvider serviceProvider = services.BuildServiceProvider();
    var fileService = serviceProvider.GetService<FileProcessingService>();

    var model = JsonSerializer.Deserialize<FileTaskModel>(message);

    var resultModel = await fileService.UploadFilePreventDuplicate(model.Content);
}

static IConfigurationRoot GetConfiguration()
{
    string appsettingsPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "appsettings.json"));

    return new ConfigurationBuilder()
        .AddJsonFile(appsettingsPath, true, true)
        .Build();
}