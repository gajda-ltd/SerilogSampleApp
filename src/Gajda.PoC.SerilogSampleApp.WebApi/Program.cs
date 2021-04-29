namespace Gajda.PoC.SerilogSampleApp.WebApi
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Sinks.Graylog;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Graylog(new GraylogSinkOptions
                {
                    HostnameOrAddress = Environment.GetEnvironmentVariable("GRAYLOG_ADDRESS") ?? "localhost",
                    Port = 12201,
                })
                .CreateLogger();

            try
            {
                Log.Information("Application is starting up...");
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}