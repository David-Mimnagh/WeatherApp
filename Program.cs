using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.AppConfiguration.AspNetCore;
using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace WeatherApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration(config =>
                    {
                        var settings = config.Build();
                        var connectionString = settings.GetConnectionString("AzureAppConfigurationEndpoint");
                        config.AddAzureAppConfiguration(options => {
                            options.Connect(connectionString)
                            .ConfigureKeyVault(kv =>
                            {
                                kv.SetCredential(new DefaultAzureCredential(true));
                            })
                            .Select(KeyFilter.Any, "BradyTech:WeatherApp");    
                        });
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    });
    }
}
