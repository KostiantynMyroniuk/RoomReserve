using Microsoft.EntityFrameworkCore;
using RoomReserve.API.Extensions;
using RoomReserve.API.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddSwaggerWithOptions();

builder.AddPersistence();
builder.AddServices();
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<ApplicationDbContext> ().Database.MigrateAsync();

    await app.SeedServices();
    await app.SeedRooms();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();
