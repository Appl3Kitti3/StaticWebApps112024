using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serverless.Models.School;

// Teddy
// G4BBYsaw

// Server=tcp:schoolgabby.database.windows.net,1433;Initial Catalog=School;Persist Security Info=False;User ID=teddy;Password=G4BBYsaw;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(x => x.AddEnvironmentVariables())
    .ConfigureHostConfiguration(x => x.AddEnvironmentVariables())
    .ConfigureServices(services =>
    {
        services.AddSingleton<HttpClient>();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<SchoolContext>(
            x =>
            {
                var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
                x.UseSqlServer(connectionString);
            }
        );
    })
    .Build();

host.Run();