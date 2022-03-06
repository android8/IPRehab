using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IPRehab
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
          //.ConfigureLogging(loggingBuilder => {
          //  loggingBuilder.ClearProviders();
          //  loggingBuilder.AddConsole();
          //})
          //.ConfigureAppConfiguration(appConfig => { 
          //  appConfig.AddIniFile("appsettings.ini");
          //})
          .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });
  }
}
