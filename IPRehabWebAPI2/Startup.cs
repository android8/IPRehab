using IPRehabModel;
using IPRehabRepository;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PatientModel;
using System.Net.Mime;
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
      string IPRehabConnectionString = Configuration.GetConnectionString("IPRehab");
      string FSODPatientConnectionString = Configuration.GetConnectionString("FSODPatientDetail");
      string MasterReportsConnectionString = Configuration.GetConnectionString("MasterReports");

      services.AddDbContext<IPRehabContext>(
         o => o.UseLazyLoadingProxies().UseSqlServer(IPRehabConnectionString));
      services.AddDbContext<DmhealthfactorsContext>(
         o => o.UseLazyLoadingProxies().UseSqlServer(FSODPatientConnectionString));
      services.AddDbContext<MasterreportsContext>(
        o => o.UseLazyLoadingProxies().UseSqlServer(MasterReportsConnectionString));

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
          Description = "A simple .Net CORE Web API for IP Rehab Patients and Uswer Access Level for filtering patient visibility",
          Contact = new OpenApiContact
          {
            Name = "C. Jonathan Sun",
            Email = "chun.sun@va.gov",
          }
        });
      });

      services.AddCors(options =>
      {
        options.AddPolicy(name: MyAllowSpecificOrigins,
                          builder =>
                          {
                            builder.WithOrigins("https://vhaausweb3.vha.med.va.gov",
                                                "https://vhaausweb3.vha.med.va.gov/iprehabmetrics",
                                                "https://localhost:44381");
                          });
      });

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
          //   //preserve circular reference
          //   o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
          //   o.JsonSerializerOptions.WriteIndented = true;
        });
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

      app.UseRouting();
      app.UseCors();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        //enforce CORS for all controlers
        endpoints.MapControllers().RequireCors(MyAllowSpecificOrigins);

        //the following line is for most apps with controllers and views, since webapi doesn't have view so it is not needed
        //endpoints.MapDefaultControllerRoute();
      });
    }
  }
}
