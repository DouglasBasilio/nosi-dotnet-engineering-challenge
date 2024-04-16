using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.API.Extensions;
using NOS.Engineering.Challenge.Database;
using System.Data;

var builder = WebApplication.CreateBuilder(args).ConfigureWebHost().RegisterServices();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var serverVersion = new MySqlServerVersion(new Version(8, 0, 26));

var connectionString = builder.Configuration.GetConnectionString("MariaDB");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion)
);

builder.Services.AddMemoryCache();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetService<ApplicationDbContext>();

    if (dbContext != null && dbContext.Database.GetDbConnection().State != ConnectionState.Open)
    {
        dbContext.Database.OpenConnection();

        // Run EnsureCreated() to create the database and any pending migrations
        dbContext.Database.EnsureCreated();
    }
}

app.MapControllers();
app.UseSwagger()
   .UseSwaggerUI();

app.Run();