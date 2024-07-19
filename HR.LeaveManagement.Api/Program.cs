using HR.LeaveManagement.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

public class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    public static void Main(string[] args)
    {
        // Ensure the 'Logs' directory exists for storing log files
        var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }

        // Build a configuration object by reading settings from 'appsettings.json' and environment variables
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json") 
            .AddEnvironmentVariables() 
            .Build();

        // Configure Serilog to use settings from the configuration object
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            Log.Information("Starting up"); 
            CreateHostBuilder(args).Build().Run(); 
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed"); 
        }
        finally
        {
            // Ensure all log entries are flushed before the application exits
            Log.CloseAndFlush(); 
        }
    }

    /// <summary>
    /// Creates a host builder for the application.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>The configured host builder.</returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
