using Athn.Helpers.MimeType;
using FileStorage;
using FileStorage.Data.MongoDb;
using FileStorage.Helpers;
using FileStorage.Helpers.Images;
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

// TODO ������ ��������?
builder.Services.AddSingleton<IFileRepository, FileRepository>();

builder.Services.AddScoped<IImageRepository, ImageRepository>();

builder.Services.AddScoped<ImageEditor>();
builder.Services.AddScoped<HashHelper>();
builder.Services.AddScoped<MimeTypeChecker>();

builder.Services.AddScoped<FileTransferService>();

builder.Services.AddScoped<IImageService, ImageService>();


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
