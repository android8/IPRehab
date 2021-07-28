using Mailer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json.Serialization;

namespace IPRehab
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<CookiePolicyOptions>(options =>
      {
        //https://docs.microsoft.com/en-us/aspnet/core/security/gdpr?view=aspnetcore-5.0
        // This lambda determines whether user consent for non-essential 
        // cookies is needed for a given request.
        options.CheckConsentNeeded = context => true; // true if consent required
        // requires using Microsoft.AspNetCore.Http;
        options.MinimumSameSitePolicy = SameSiteMode.None;
        options.ConsentCookie.Expiration = TimeSpan.FromMinutes(30);
      });
      services.AddSession(options =>
      {
        options.IdleTimeout = TimeSpan.FromMinutes(20);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
      });

      //services.AddDistributedMemoryCache();
      services.AddMemoryCache();
      services.AddControllersWithViews().AddJsonOptions(o =>
      {
           //preserve circular reference
           o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
      });
      services.AddResponseCaching();
      services.AddRazorPages();
      services.AddSingleton<IMailerConfiguration, MailerConfiguration>();
      services.AddSingleton<IEmailSender, EmailSender>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }
      app.UseHttpsRedirection();
      app.UseStaticFiles();
      
      //https://docs.microsoft.com/en-us/aspnet/core/security/gdpr?view=aspnetcore-5.0
      app.UseCookiePolicy();
      
      app.UseRouting();
      //DB connection and Identty is handle in Areas.Identity.IdentityHostingStartup.cs
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseSession(); //must be before UseMvc or UseEndpoints

      // UseCors must be called before UseResponseCaching
      // app.UseCors("myAllowSpecificOrigins");

      app.UseResponseCaching();

      app.Use(async (context, next) =>
      {
        context.Response.GetTypedHeaders().CacheControl =
            new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
            {
              Public = true,
              MaxAge = TimeSpan.FromSeconds(10)
            };
        context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
            new string[] { "Accept-Encoding" };

        await next();
      });

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Patient}/{action=Index}/{criteria?}");

        endpoints.MapRazorPages();
      });
    }
  }
}
