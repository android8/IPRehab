using IPRehabModel;
using IPRehabRepository;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Filters;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PatientModel_TreatingSpecialty;
using System.Net.Mime;
using System.Text.Json;
using UserModel;

namespace IPRehabWebAPI2
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            #region custom appsettings

            //https://www.bing.com/ck/a?!&&p=9f40ffcae2103a86JmltdHM9MTY2MjQyMjQwMCZpZ3VpZD0yMGE2ODFkNi05OTNiLTZiN2YtMThjYS05M2MxOThiZjZhNTEmaW5zaWQ9NTQ0MA&ptn=3&hsh=3&fclid=20a681d6-993b-6b7f-18ca-93c198bf6a51&u=a1aHR0cHM6Ly93d3cuYy1zaGFycGNvcm5lci5jb20vYXJ0aWNsZS9yZWFkaW5nLXZhbHVlcy1mcm9tLWFwcHNldHRpbmdzLWpzb24taW4tYXNwLW5ldC1jb3JlLyM6fjp0ZXh0PVRoZXJlJTIwYXJlJTIwdHdvJTIwbWV0aG9kcyUyMHRvJTIwcmV0cmlldmUlMjBvdXIlMjB2YWx1ZXMlMkMsYXJlJTIwZ2V0dGluZyUyMGFub3RoZXIlMjBzZWN0aW9uJTIwdGhhdCUyMGNvbnRhaW5zJTIwdGhlJTIwdmFsdWUu&ntb=1
            services.Configure<CustomAppSettingsModel>(Configuration.GetSection("AppSettings"));

            #endregion custom appsettings

            #region db setup

            string IPRehabConnectionString = Configuration.GetConnectionString("IPRehab");
            string MasterReportsConnectionString = Configuration.GetConnectionString("MasterReports");
            string TreatingSpecialtyConnectionString = Configuration.GetConnectionString("TreatingSpecialty");

            //register the internal IPRehab DB context
            //UseLoggerFactory output in debug window
            services.AddDbContext<IPRehabContext>(
               o => o.UseLazyLoadingProxies()
               //.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
               .UseSqlServer(IPRehabConnectionString));

            //register the external Masterreports DB context for users
            services.AddDbContext<MasterreportsContext>(
              o => o.UseLazyLoadingProxies()
              //.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
              .UseSqlServer(MasterReportsConnectionString));

            //register the external TreatingSpecialty DB context for users
            services.AddDbContext<DMTreatingSpecialtyContext>(
              o => o.UseLazyLoadingProxies()
              //.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
              .UseSqlServer(TreatingSpecialtyConnectionString));


            #endregion db setup

            #region IoC container

            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<ICodeSetRepository, CodeSetRepository>();
            services.AddScoped<IEpisodeOfCareRepository, EpisodeOfCareRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IQuestionInstructionRepository, QuestionInstructionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISignatureRepository, SignatureRepository>();
            services.AddScoped<IQuestionMeasureRepository, QuestionMeasureRepository>();

            //treating specialty Non-DIRECT uses a link server sql view in DB2 to get patients via IPRehabContext
            services.AddScoped<ITreatingSpecialtyPatientRepository, TreatingSpecialtyPatientRepository>();

            //treating specialty DIRECT gets patients directly from BI13 using the DMTreatingSpecialtyContext
            services.AddScoped<ITreatingSpecialtyDirectPatientRepository, TreatingSpecialtyDirectPatientRepository>();

            //https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-providers

            //UserPatientCacheHelper deals with vTreatingSpecialtyRecent3Yrs to DTO mapping
            services.AddScoped<IUserPatientCacheHelper, UserPatientCacheHelper>();

            //UserPatientCacheHelper_TreatingSpecialty deals with RptRehabDetails to DTO mapping
            services.AddScoped<IUserPatientCacheHelper_TreatingSpecialty, UserPatientCacheHelper_TreatingSpecialty>();

            //TreatingSpecialtyPatientDemographicRepository deals with patients not in RptRehabDetails to DTO mapping
            services.AddScoped<ITreatingSpecialtyPatientDemographicRepository, TreatingSpecialtyPatientDemographicRepository>();

            services.AddScoped<AnswerHelper, AnswerHelper>();

            #endregion IoC

            #region SWAGGER interface

            /* https://docs.microsoft.com/en-us/samples/dotnet/aspnetcore.docs/getstarted-swashbuckle-aspnetcore/?tabs=visual-studio
             * In properties\launchSettimgs.json set 
             * "launchUrl": "api/TreatingSpecialtyPatient to start with TreatingSpecialtyPatient patient page
             * "launchUrl": "swagger" to start with Swagger interface */

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v5", new OpenApiInfo
                {
                    Title = "IPRehabWebAPI2",
                    Version = "v5",
                    Description = "A .Net 7 Web API and Entity Framework CORE servicing in patient rehab using Treating Specialty data",
                    Contact = new OpenApiContact
                    {
                        Name = "C. Jonathan Sun",
                        Email = "chun.sun@va.gov",
                    }
                });
            });

            #endregion SWAGGER

            #region CORS

            //services.AddCors();
            services.AddCors(options =>
            {
                //use default policy
                options.AddDefaultPolicy(builder => builder
                    .WithOrigins("https://localhost:44381",
                                 "https://vhaausweb3.vha.med.va.gov",
                                 "https://vaww.vssc.med.va.gov",
                                 "https://secure.vssc.med.va.gov")
                    .AllowAnyHeader().AllowAnyMethod());

                //use custom policy
                //options.AddPolicy(
                //name: MyAllowSpecificOrigins,
                //policyBuilder =>
                //{
                //    policyBuilder
                //    //The allowed URL must not contain a trailing slash (/)
                //    .WithOrigins("https://localhost:44381",
                //                  "https://vhaausweb3.vha.med.va.gov",
                //                  "https://vaww.vssc.med.va.gov",
                //                  "https://secure.vssc.med.va.gov"
                //                  )
                //    .AllowAnyOrigin() //comment out for testing on localhost
                //    .AllowAnyHeader()
                //    .AllowAnyMethod();
                //});
            });

            #endregion CORS

            #region authentication authorization
            //https://learn.microsoft.com/en-us/aspnet/core/security/authentication/windowsauth?view=aspnetcore-8.0&tabs=netcore-cli
            //services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddAuthentication(options => { options.DefaultAuthenticateScheme = options.DefaultAuthenticateScheme; }).AddNegotiate();
            services.AddAuthorization(options => { options.FallbackPolicy = options.DefaultPolicy; });
            #endregion

            #region API Controller behaviors

            services.AddControllers(o =>
            {
                o.EnableEndpointRouting = true;

                /* Use exceptions to modify the response
                The contents of the response can be modified from outside of the controller.
                In ASP.NET 4.x Web API, one way to do this was using the HttpResponseException type. 
                ASP.NET Core doesn't include an equivalent type. */
                o.Filters.Add(new HttpResponseExceptionFilter());
            })

            /* For web API controllers, MVC responds with a ValidationProblemDetails response type 
             * when model validation fails. MVC uses the results of InvalidModelStateResponseFactory 
             * to construct the error response for a validation failure. The following example uses 
             * the factory to change the default response type to SerializableError. */
            .ConfigureApiBehaviorOptions(options =>
            {
                //The automatic creation of a ProblemDetails for error status codes is disabled when the SuppressMapClientErrors property is set to true:
                options.SuppressMapClientErrors = false;

                options.InvalidModelStateResponseFactory = context =>
                    new BadRequestObjectResult(context.ModelState)
                    {
                        ContentTypes = {
                            // using static System.Net.Mime.MediaTypeNames;
                            MediaTypeNames.Application.Json,
                            MediaTypeNames.Application.Xml
                        }
                    };
            })
            .AddXmlSerializerFormatters()
            .AddJsonOptions(o =>
              {
                  //preserve circular reference
                  //o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                  o.JsonSerializerOptions.WriteIndented = true;
                  o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
              });
            //services.AddProblemDetails(); //only avaialbe in .Net 7 or newer
            //services.AddTransient<ProblemDetailsFactory, SampleProblemDetailsFactory>();

            #endregion API Controller behaviors
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //https://medium.com/@niteshsinghal85/how-i-have-handled-exception-globally-in-asp-net-core-api-7b8c2603af72
            app.ConfigureExceptionHandler(env); //use ExceptionMiddleware extension of IApplicationBuilder

            app.UseStatusCodePages();

            string apiAppSettings = env.IsDevelopment() ? "D" : string.Empty;

            //if (env.IsDevelopment())
            //{
            //    //app.UseDeveloperExceptionPage();

            //    //or use custom Error controller
            //    app.UseExceptionHandler("/error-local-development");
            //}
            //else
            //{
            //    app.UseExceptionHandler("/error");
            //}

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
#if DEBUG
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                // For Debug in Kestrel
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v5/swagger.json", $"Web API V5 {apiAppSettings}");
#else
                // To deploy on IIS
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v5/swagger.json", $"Web API V5 {apiAppSettings}");
#endif
                //To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string:
                //c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            if (env.IsProduction())
            {
                //UseCors() must be placed after UseRouting, but before UseAuthorization and must call before UseResponseCaching, if used.
                app.UseCors(MyAllowSpecificOrigins);
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //enforce CORS for all web api controlers without [CORS] attribute
                endpoints.MapControllers();//.RequireCors(MyAllowSpecificOrigins);

                //the following line is for most apps with controllers and views, since webapi doesn't have view so it is not needed
                //endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
