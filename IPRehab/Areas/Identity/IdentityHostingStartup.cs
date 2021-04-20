using System;
using IPRehabModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
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
            builder.ConfigureServices((context, services) => {
               services.AddDbContext<IPRehabContext>(options =>
                  options.UseSqlServer(
                     context.Configuration.GetConnectionString("IPRehab"),
                     options => options.MigrationsAssembly("IPRehabModel")
                  )
               );

               services.AddDefaultIdentity<ApplicationUser>(options =>
                  options.SignIn.RequireConfirmedAccount = true)
               .AddEntityFrameworkStores<IPRehabContext>();
            });
        }
    }
}