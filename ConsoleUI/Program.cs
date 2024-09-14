using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

// DI, Serilog, Settings


var builder = new ConfigurationBuilder();
BuildConfig(builder);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Build())
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();


Log.Logger.Information("Application Starting");

// for dependency injection
// this is setting up default hierachy for default configuration it configures user secrets and so on
// it enables scope validation on the dependency injection container when environment name is development
var host = Host.CreateDefaultBuilder()
    // place to store all your dependencies
    .ConfigureServices((context, services) =>
    {
        // write all services here 
        services.AddTransient<IGreetingService, GreetingService>();
    })
    .UseSerilog()
    // builds our Ibuilder and put into host, it has our logging DI and services
    .Build();

// so everything is in host so we just call in host services find IGreetingServices and we get class instantiation
var svc = ActivatorUtilities.CreateInstance<IGreetingService>(host.Services);
// it will call this run method in class 
svc.Run();



static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        // this is get production or development json and it gets it with all settings
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
        .AddEnvironmentVariables();
}
