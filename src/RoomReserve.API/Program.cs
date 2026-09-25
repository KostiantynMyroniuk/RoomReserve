using RoomReserve.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

builder.AddPersistence();
builder.AddServices();
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();
