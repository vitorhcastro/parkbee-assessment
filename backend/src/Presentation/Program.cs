using Application;
using Infrastructure;
using Presentation.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddControllersConfiguration();
builder.Services.AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AddSeedData();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
