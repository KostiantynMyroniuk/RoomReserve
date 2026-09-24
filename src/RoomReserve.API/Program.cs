using RoomReserve.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddPersistence();
builder.AddServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
