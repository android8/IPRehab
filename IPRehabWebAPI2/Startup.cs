using IPRehabModel;
using IPRehabRepository;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Filters;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PatientModel;
using System.Net.Mime;
using System.Text.Json;
using UserModel;

namespace IPRehabWebAPI2
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region custom appsettins
            //https://www.bing.com/ck/a?!&&p=9f40ffcae2103a86JmltdHM9MTY2MjQyMjQwMCZpZ3VpZD0yMGE2ODFkNi05OTNiLTZiN2YtMThjYS05M2MxOThiZjZhNTEmaW5zaWQ9NTQ0MA&ptn=3&hsh=3&fclid=20a681d6-993b-6b7f-18ca-93c198bf6a51&u=a1aHR0cHM6Ly93d3cuYy1zaGFycGNvcm5lci5jb20vYXJ0aWNsZS9yZWFkaW5nLXZhbHVlcy1mcm9tLWFwcHNldHRpbmdzLWpzb24taW4tYXNwLW5ldC1jb3JlLyM6fjp0ZXh0PVRoZXJlJTIwYXJlJTIwdHdvJTIwbWV0aG9kcyUyMHRvJTIwcmV0cmlldmUlMjBvdXIlMjB2YWx1ZXMlMkMsYXJlJTIwZ2V0dGluZyUyMGFub3RoZXIlMjBzZWN0aW9uJTIwdGhhdCUyMGNvbnRhaW5zJTIwdGhlJTIwdmFsdWUu&ntb=1
            services.Configure<CustomAppSettingsModel>(Configuration.GetSection("AppSettings"));
            #endregion

            #region db setup
            string IPRehabConnectionString = Configuration.GetConnectionString("IPRehab");
            string FSODPatientConnectionString = Configuration.GetConnectionString("FSODPatientDetail");
            string MasterReportsConnectionString = Configuration.GetConnectionString("MasterReports");

            //register the internal IPRehab DB context
            services.AddDbContext<IPRehabContext>(
               o => o.UseLazyLoadingProxies().UseSqlServer(IPRehabConnectionString));

            //register the external Dmhealthfactors DB context for patients
            services.AddDbContext<DmhealthfactorsContext>(
               o => o.UseLazyLoadingProxies().UseSqlServer(FSODPatientConnectionString));

            //register the external Masterreports DB context for users
            services.AddDbContext<MasterreportsContext>(
              o => o.UseLazyLoadingProxies().UseSqlServer(MasterReportsConnectionString));
            #endregion

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
            services.AddScoped<IFSODPatientRepository, FSODPatientRepository>();
            services.AddScoped<ITreatingSpecialtyPatientRepository, TreatingSpecialtyPatientRepository>();
            services.AddScoped<IUserPatientCacheHelper, UserPatientCacheHelper>();

            /* ToDO: to be deleted after test */
            services.AddScoped<ITestUserPatientCacheHelper, TestUserPatientCacheHelper>();

            services.AddScoped<AnswerHelper, AnswerHelper>();
            #endregion

            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            #region SWAGGER interface
            /* https://docs.microsoft.com/en-us/samples/dotnet/aspnetcore.docs/getstarted-swashbuckle-aspnetcore/?tabs=visual-studio
             * In properties\launchSettimgs.json set 
             * "launchUrl": "api/TreatingSpecialtyPatient to start with TreatingSpecialtyPatient patient page
             * "launchUrl": "api/FSODPatient" to start with FSODPatient page 
             * "launchUrl": "swagger" to start with Swagger interface */

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v4", new OpenApiInfo
                {
                    Title = "IPRehabWebAPI2",
                    Version = "v4",
                    Description = "A .Net 6 Web API and EF CORE 6 servicing in patient rehab using Treating Specialty data",
                    Contact = new OpenApiContact
                    {
                        Name = "C. Jonathan Sun",
                        Email = "chun.sun@va.gov",
                    }
                });
            });
            #endregion

            #region CORS
            //services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: MyAllowSpecificOrigins,
                    policyBuilder =>
                    {
                        policyBuilder
                        .WithOrigins("https://localhost:44381", "https://vhaausweb3.vha.med.va.gov", "https://https://vaww.vssc.med.va.gov/")
                        //.AllowAnyOrigin() //comment out for testing on localhost
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });
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
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new BadRequestObjectResult(context.ModelState);

                    result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                    return result;
                };
            })
            .AddJsonOptions(o =>
              {
                  //preserve circular reference
                  //o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                  o.JsonSerializerOptions.WriteIndented = true;
                  o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
              });
            #endregion
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
                app.UseExceptionHandler("/error");
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
#if DEBUG
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                // For Debug in Kestrel
                c.SwaggerEndpoint("/swagger/v4/swagger.json", "Web API V4");
#else
            // To deploy on IIS
            c.SwaggerEndpoint("/iprehabmetricswebapi/swagger/v4/swagger.json", "Web API V4");
#endif
                //To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string:
                //c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            if (env.IsProduction())
            {
                //UseCors() must be btween UseRouting and UseAuthorization
                app.UseCors(MyAllowSpecificOrigins);
            }

            //app.UseAuthentication();
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
