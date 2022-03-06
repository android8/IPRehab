using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace IPRehabWebAPI2
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            //.ConfigureLogging(loggingBuilder =>
            //{
            //  loggingBuilder.ClearProviders();
            //  loggingBuilder.AddConsole();
            //})
            //.ConfigureAppConfiguration(appConfig =>
            //{
            //  appConfig.AddIniFile("appsettings.ini");
            //})
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseKestrel();
              webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
              webBuilder.UseIISIntegration();
              webBuilder.UseIIS();
              webBuilder.UseStartup<Startup>();
            });
  }


}
