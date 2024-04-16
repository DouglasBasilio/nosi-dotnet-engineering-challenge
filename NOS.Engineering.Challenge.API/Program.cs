using NOS.Engineering.Challenge.API.Extensions;

var builder = WebApplication.CreateBuilder(args).ConfigureWebHost().RegisterServices();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var app = builder.Build();

app.MapControllers();
app.UseSwagger()
   .UseSwaggerUI();

app.Run();