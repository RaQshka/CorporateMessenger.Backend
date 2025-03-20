using System.Reflection;
using Messenger.Application;
using Messenger.Application.Common.Mappings;
using Messenger.Application.Interfaces;
using Messenger.Persistence;
using Messenger.Persistence.Migrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Notes.WebApi
{
    public class Startup
    {
        public IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
                config.AddProfile(new AssemblyMappingProfile(typeof(IMessengerDbContext).Assembly));
            });
            //services.AddApplication();
            services.AddPersistance(_configuration);
            services.AddControllers();
            services.AddTransient<MessengerDbContextFactory>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });

            /*services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("Bearer", options =>
            {
                options.Authority = "http://localhost:44322";
                options.Audience = "NotesWebAPI";
                options.RequireHttpsMetadata = false;
            });
            services.AddVersionedApiExplorer(config => config.GroupNameFormat = "'v'VVV");

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(x =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });*/

            /*services.AddApiVersioning();*/
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env/*, 
            IApiVersionDescriptionProvider provider*/)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            /*app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                foreach(var description in provider.ApiVersionDescriptions)
                {
                    config.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    config.RoutePrefix = String.Empty;
                }
            });

            app.UseCustomExceptionBuilder();*/
            app.UseRouting();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(e => e.MapControllers());
        }
    }
}
