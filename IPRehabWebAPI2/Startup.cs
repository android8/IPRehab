using IPRehabModel;
using IPRehabRepository;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Filters;
using IPRehabWebAPI2.Helpers;
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
      services.AddScoped<IQuestionStageRepository, QuestionStageRepository>();
      services.AddScoped<IFSODPatientRepository, FSODPatientRepository>();
      services.AddScoped<IUserPatientCacheHelper, UserPatientCacheHelper>();

      services.AddScoped<AnswerHelper, AnswerHelper>();
      #endregion

      #region API Controller behaviors
      services.AddControllers(o =>
      {
        o.EnableEndpointRouting = true;

        /* Use exceptions to modify the response
          The contents of the response can be modified from outside of the controller. In ASP.NET 4.x Web API, one way to do this was using the HttpResponseException type. ASP.NET Core doesn't include an equivalent type. */
        o.Filters.Add(new HttpResponseExceptionFilter());
      })

      /* For web API controllers, MVC responds with a ValidationProblemDetails response type when model validation fails. MVC uses the results of InvalidModelStateResponseFactory to construct the error response for a validation failure. The following example uses the factory to change the default response type to SerializableError. */
      .ConfigureApiBehaviorOptions(options =>
      {
        options.InvalidModelStateResponseFactory = context =>
        {
          var result = new BadRequestObjectResult(context.ModelState);

          // TODO: add `using System.Net.Mime;` to resolve MediaTypeNames
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

      services.AddAuthentication(IISDefaults.AuthenticationScheme);

      #region SWAGGER interface
      /* https://docs.microsoft.com/en-us/samples/dotnet/aspnetcore.docs/getstarted-swashbuckle-aspnetcore/?tabs=visual-studio */

      /* set "launchUrl": "api/FSODPatient" in properties\launchSettimgs.json to start with FSODPatient page
             "launchUrl": "swagger" to start with Swagger interface
      */
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v3", new OpenApiInfo
        {
          Title = "IPRehabWebAPI2",
          Version = "v3",
          Description = "A simple .Net 5 Web API for IP Rehab Patients and Uswer Access Level for filtering patient visibility",
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
        options.AddPolicy(MyAllowSpecificOrigins, builder =>
                          {
                            // The specified URL must not contain a trailing slash (/)
                            builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                            //.SetIsOriginAllowed(origin => true)
                            //.SetIsOriginAllowedToAllowWildcardSubdomains()

                            //.WithOrigins("https://localhost:44381")
                            //.AllowCredentials();
                          });
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
        c.SwaggerEndpoint("/swagger/v3/swagger.json", "Web API V3");
#else
            // To deploy on IIS
            c.SwaggerEndpoint("/iprehabmetricswebapi/swagger/v3/swagger.json", "Web API V3");
#endif
        //To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string:
        //c.RoutePrefix = string.Empty;
      });

      app.UseHttpsRedirection();

      //The call to UseCors must be placed after UseRouting, but before UseAuthorization
      app.UseRouting();

      app.UseCors(MyAllowSpecificOrigins);
      //app.UseCors();

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
