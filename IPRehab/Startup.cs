using IPRehabRepository.Contracts;
using IPRehabRepository;
using IPRehabWebAPI2.Helpers;
using Mailer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;
using UserModel;

namespace IPRehab
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Env = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                //https://docs.microsoft.com/en-us/aspnet/core/security/gdpr?view=aspnetcore-5.0

                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true; // true if consent required
                                                              // requires using Microsoft.AspNetCore.Http;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookie.Expiration = TimeSpan.FromMinutes(20);
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //services.AddDistributedMemoryCache(); //do not use because out of process 
            services.AddMemoryCache(); //do not use because the server may not have the same DLL versions
            services.AddResponseCaching();

            services.AddControllersWithViews()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; //preserve circular reference
                })
                .AddMvcOptions(options =>
                {
                    options.CacheProfiles.Add("PrivateCache",
                        new CacheProfile() { Duration = 60 * 24, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new[] {"*"} });
                });

            #region web optimization
            // https://github.com/ligershark/WebOptimizer
            if (Env.IsDevelopment())
            {
                /* no minification */
                services.AddWebOptimizer(minifyJavaScript: false, minifyCss: false);
            }
            else
            {
                services.AddWebOptimizer(pipeline =>
                {
                    // Creates a CSS and a JS bundle. Globbing patterns supported.
                    pipeline.AddScssBundle("/css/siteCssBundle.css", new string[] {
                      "css/**/site.css",
                      "css/**/transition.css",
                      "css/**/app.css"
                    });

                    pipeline.AddJavaScriptBundle("/js/siteJsBundle.js", new string[] {
                      "js/**/animateBanner.js",
                      "js/**/cookieConsent.js"});

                    pipeline.AddJavaScriptBundle("/js/patientBundle.js", new string[] {
                      "js/**/commandBtns.js",
                      "js/**/patientList.js"});

                    pipeline.AddJavaScriptBundle("/js/questionBundle.js", new string[] {
                      "js/**/commandBtns.js",
                      "js/**/branching.js",
                      "js/**/form.js" });

                    // This bundle uses source files from the Content Root and uses a custom PrependHeader extension
                    //pipeline.AddJavaScriptBundle("/js/scripts.js", "scripts/a.js", "wwwroot/js/plus.js")
                    //        .UseContentRoot()
                    //        .PrependHeader("My custom header")
                    //        .AddResponseHeader("x-test", "value");

                    // This will minify any JS and CSS file that isn't part of any bundle
                    pipeline.MinifyCssFiles();
                    pipeline.MinifyJsFiles();

                    // This will automatically compile any referenced .scss files
                    pipeline.CompileScssFiles();

                    // AddFiles/AddBundle allow for custom pipelines
                    pipeline.AddBundle("/text.txt", "text/plain", "random/*.txt")
                    .AdjustRelativePaths()
                    .Concatenate()
                    .FingerprintUrls()
                    .MinifyCss();
                });
            }
            #endregion

            services.AddRazorPages();

            //register the external Masterreports DB context for users
            string MasterReportsConnectionString = Configuration.GetConnectionString("MasterReports");

            services.AddDbContext<MasterreportsContext>(
              o => o.UseLazyLoadingProxies()
              .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
              .UseSqlServer(MasterReportsConnectionString));

            #region IoC
            /* IoC for mailer.  It's not WebAPI's concern so it should not register there */
            services.AddSingleton<IMailerConfiguration, MailerConfiguration>();
            services.AddSingleton<Mailer.IEmailSender, EmailSender>();

            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<ICodeSetRepository, CodeSetRepository>();
            services.AddScoped<IEpisodeOfCareRepository, EpisodeOfCareRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IQuestionInstructionRepository, QuestionInstructionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISignatureRepository, SignatureRepository>();
            services.AddScoped<IQuestionMeasureRepository, QuestionMeasureRepository>();
            services.AddScoped<ITreatingSpecialtyPatientRepository, TreatingSpecialtyPatientRepository>();
            services.AddScoped<IUserPatientCacheHelper, UserPatientCacheHelper>();
            #endregion

            /* Implement ProblemDetailsFactory
              MVC uses Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory to produce all instances of ProblemDetails and ValidationProblemDetails. This includes client error responses, validation failure error responses, and the ControllerBase.Problem and ControllerBase.ValidationProblem helper methods.
             */
            /* https://stevenmaglio.blogspot.com/2019/12/create-custom-problemdetailsfactory.html */

            //services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN-IPREHAB";
                options.FormFieldName = "X-CSRF-TOKEN-IPREHAB";
                options.Cookie.Name = "X-CSRF-TOKEN-IPREHAB";
            });

            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Exception Handler
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();

                //route to ErrorController action that has [Route("/error-local-development")]
                app.UseExceptionHandler("/error-local-development");
            }
            else
            {
                //route to ErrorController action that has [Route("/error")]
                app.UseExceptionHandler("/error");

                //app.UseExceptionHandler(appBuilder =>
                //{
                //  override the current Path
                //  appBuilder.Use(async (ctx, next) =>
                //  {
                //      ctx.Request.Path = "/error";
                //      await next();
                //  });

                //  let the staticFiles middleware to serve the sth-wrong.html
                //  appBuilder.UseStaticFiles();
                //});

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            #endregion

            app.UseHttpsRedirection();

            app.UseWebOptimizer();/* must be before UseStaticFiles() */

            app.UseStaticFiles();

            //https://docs.microsoft.com/en-us/aspnet/core/security/gdpr?view=aspnetcore-5.0
            app.UseCookiePolicy();

            app.UseRouting();

            //DB connection and Identty is handled in Areas.Identity.IdentityHostingStartup.cs

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession(); //must be before UseMvc or UseEndpoints

            app.UseResponseCaching();
            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(10)
                };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new string[] { "Accept-Encoding" };

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Splash}");

                endpoints.MapRazorPages();
            });
        }
    }
}
