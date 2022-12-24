using FileStorage;
using FileStorage.Data.MongoDb;
using FileStorage.Server.MinimalWebAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

builder.Services.Configure<DatabaseSettings>(
   builder.Configuration.GetSection(nameof(DatabaseSettings)));

builder.Services.AddSingleton<IDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

builder.Services.AddSingleton<IFileRepository, FileRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/files/{id}", async (string id, IFileRepository service) =>
{
    var file = await service?.DownLoadFileAsync(id);
    return Results.File(file.Content);
});

app.Run();
