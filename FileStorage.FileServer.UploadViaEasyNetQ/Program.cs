// See https://aka.ms/new-console-template for more information
using Athn.Helpers.MimeType;
using EasyNetQ;
using FileStorage;
using FileStorage.Data.MongoDb;
using FileStorage.FileServer.UploadViaEasyNetQ;
using FileStorage.Helpers;
using FileStorage.Models;
using FileStorage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

Console.WriteLine("Hello, World!");

IConfigurationRoot config = GetConfiguration();
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

using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
    await bus.Rpc.RespondAsync<FileTaskModel, StoredFileModel>(HandleMessage);
    Console.WriteLine("Listening for messages. Hit <return> to quit.");
    Console.ReadLine();
}

async Task<StoredFileModel> HandleMessage(FileTaskModel textMessage)
{
    ServiceProvider serviceProvider = services.BuildServiceProvider();
    var fileService = serviceProvider.GetService<FileProcessingService>();

    var resultModel = await fileService.UploadFilePreventDuplicate(textMessage.Content);

    return resultModel;
}

IConfigurationRoot GetConfiguration()
{
    string appsettingsPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "appsettings.json"));

    return new ConfigurationBuilder()
        .AddJsonFile(appsettingsPath, true, true)
        .Build();
}