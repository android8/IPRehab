using IPRehabModel;
using IPRehabRepository;
using IPRehabRepository.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PatientModel;
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

      /*set "launchUrl": "api/FSODPatient" in properties\launchSettimgs.json to start with FSODPatient page
       * "launchUrl": "swagger" to start with Swagger interface
       */
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v2", new OpenApiInfo { Title = "IPRehabWebAPI2", Version = "v2" });
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

      services.AddControllers(o => o.EnableEndpointRouting = true)
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
      if (env.IsDevelopment() || env.IsProduction())
      {
        app.UseDeveloperExceptionPage();
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
          #if DEBUG
            // For Debug in Kestrel
            c.SwaggerEndpoint("/swagger/v2/swagger.json", "Web API V2");
          #else
            // To deploy on IIS
            c.SwaggerEndpoint("/iprehabmetricswebapi/swagger/v2/swagger.json", "Web API V2");
          #endif
          //c.RoutePrefix = string.Empty;
        });
      }

      app.UseHttpsRedirection();

      app.UseRouting();
      app.UseCors();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers().RequireCors(MyAllowSpecificOrigins);
        //for most apps with controllers and views endpoints.MapDefaultControllerRoute();
      });
    }
  }
}
