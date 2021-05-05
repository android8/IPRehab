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
using System.Text.Json.Serialization;

namespace IPRehabWebAPI2
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
         /* inject EFCore */
         //services.AddDbContext<TodoContext>(
         //  opt => opt.UseInMemoryDatabase("TodoList"));

         string thisConnectionString = Configuration.GetConnectionString("IPRehab");
         services.AddDbContext<IPRehabContext>(
                  options => options.UseLazyLoadingProxies().UseSqlServer(
                        thisConnectionString)
                  );

         services.AddScoped<IQuestionRepository, QuestionRepository>();
         services.AddScoped<IAnswerRepository, AnswerRepository>();
         services.AddScoped<ICodeSetRepository, CodeSetRepository>();
         services.AddScoped<IEpisodeOfCareRepository, EpisodeOfCareRepository>();
         services.AddScoped<IPatientRepository, PatientRepository>();
         services.AddScoped<IQuestionInstructionRepository, QuestionInstructionRepository>();
         services.AddScoped<IUserRepository, UserRepository>();
         services.AddScoped<ISignatureRepository, SignatureRepository>();
         services.AddScoped<IQuestionStageRepository, QuestionStageRepository>();

         services.AddControllers().AddJsonOptions(o =>
         {
            //preserve circular reference
            o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            o.JsonSerializerOptions.WriteIndented = true;
         });

         /*set "launchUrl": "api/TodoItems" in properties\launchSettimgs.json to start with TodoItems page
          * "launchUrl": "swagger" to start with Swagger interface
          */
         services.AddSwaggerGen(c =>
         {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "IPRehabWebAPI2", Version = "v1" });
         });
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IPRehabWebAPI2 v1"));
         }

         app.UseHttpsRedirection();

         app.UseRouting();

         app.UseAuthorization();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapControllers(); //for most apps with controllers and views 
            //endpoints.MapDefaultControllerRoute();
         });
      }
   }
}
