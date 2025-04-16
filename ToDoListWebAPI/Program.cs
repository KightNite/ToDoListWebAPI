using Microsoft.EntityFrameworkCore;
using ToDoListWebAPI.Models;
using ToDoListWebAPI.Repository.Interfaces;
using ToDoListWebAPI.Repository.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.AddConsole();
    options.AddDebug();
});
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument();
builder.Services.AddDbContext<ToDoContext>(opt => 
    opt.UseInMemoryDatabase("ToDoList"));
builder.Services.AddScoped<IToDoItemRepository, ToDoItemRepository>();
builder.Services.AddScoped<IToDoListRepository, ToDoListRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();