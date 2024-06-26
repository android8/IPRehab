﻿using IPRehabModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(NoIdentity.Areas.Identity.IdentityHostingStartup))]
namespace NoIdentity.Areas.Identity
{
   public class IdentityHostingStartup : IHostingStartup
   {
      public void Configure(IWebHostBuilder builder)
      {
         builder.ConfigureServices((context, services) =>
         {
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