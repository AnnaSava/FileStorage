using Athn.Helpers.MimeType;
using FileStorage;
using FileStorage.Data.MongoDb;
using FileStorage.Helpers;
using FileStorage.Helpers.Images;
using FileStorage.Models;
using FileStorage.Server.WebAPI;
using FileStorage.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// requires using Microsoft.Extensions.Options
builder.Services.Configure<DatabaseSettings>(
   builder.Configuration.GetSection(nameof(DatabaseSettings)));

builder.Services.AddSingleton<IDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

builder.Services.Configure<FileServerSettings>(
   builder.Configuration.GetSection(nameof(FileServerSettings)));

builder.Services.AddSingleton<FileServerSettings>(sp => sp.GetRequiredService<IOptions<FileServerSettings>>().Value);


builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMq"));

// TODO Почему синглтон?
builder.Services.AddSingleton<IFileRepository, FileRepository>();

builder.Services.AddScoped<IImageRepository, ImageRepository>();

builder.Services.AddScoped<ImageEditor>();
builder.Services.AddScoped<HashHelper>();
builder.Services.AddScoped<MimeTypeChecker>();

var imageServiceConf = builder.Configuration.GetSection("AppConfiguration:ImageServiceConfig")?.Value;

switch (imageServiceConf)
{
    case "ImageFileMonolithService":
        builder.Services.AddScoped<FileProcessingService>();
        builder.Services.AddScoped<IImageService, ImageService>();
        break;
    case "ImageFileTransferService":
        builder.Services.AddScoped<FileTransferService>();
        builder.Services.AddScoped<IImageService, ImageFileTransferService>();
        break;
    case "ImageFileQueueService":
        builder.Services.AddScoped<FileQueueService>();
        builder.Services.AddScoped<FileEasyNetQueueService>();
        builder.Services.AddScoped<IImageService, ImageFileQueueService>();
        break;
    default:
        break;
}


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("https://localhost:7099");  //set the allowed origin
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
         });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
