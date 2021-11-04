using IPRehabModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(IPRehab.Areas.Identity.IdentityHostingStartup))]
namespace IPRehab.Areas.Identity
{
  public class IdentityHostingStartup : IHostingStartup
  {
    public void Configure(IWebHostBuilder builder)
    {
      //use IPRehabContext as the custom CORE identity
      builder.ConfigureServices((context, services) =>
      {
        services.AddDbContext<IPRehabContext>(options =>
           options.UseLazyLoadingProxies()  /* default lazy loading */
              .UseSqlServer(
                 context.Configuration.GetConnectionString("IPRehab"),
                 options => options.MigrationsAssembly("IPRehabModel")
              )
           );

        services.AddDefaultIdentity<ApplicationUser>(options =>
           options.SignIn.RequireConfirmedAccount = false)
        .AddEntityFrameworkStores<IPRehabContext>();
      });
    }
  }
}