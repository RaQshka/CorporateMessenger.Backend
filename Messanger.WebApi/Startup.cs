using System.Reflection;
using System.Text;
using Messenger.Application;
using Messenger.Application.Common.Mappings;
using Messenger.Application.Interfaces;
using Messenger.Persistence;
using Messenger.Persistence.Migrations;
using Messenger.WebApi.Hubs;
using Messenger.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Messenger.WebApi
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

            services.AddApplication();
            services.AddPersistance(_configuration);
            services.AddControllers();            
            /*services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN"; // Имя заголовка для токена
            });*/
            services.AddHttpContextAccessor();
            services.AddTransient<MessengerDbContextFactory>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); 
                });
            });
            var jwtSettings = _configuration.GetSection("JwtSettings");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ClockSkew = TimeSpan.Zero // Без задержки истечения токена
                };
            });
            services.AddSignalR();
            services.AddAuthorization();
            services.AddEndpointsApiExplorer();
            
            

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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "MyApi", 
                    Version = "v1" 
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    In = ParameterLocation.Header, 
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey 
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    { 
                        new OpenApiSecurityScheme 
                        { 
                            Reference = new OpenApiReference 
                            { 
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" 
                            } 
                        },
                        new string[] { } 
                    } 
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env /*,
            IApiVersionDescriptionProvider provider*/)
        {
            if (env.IsDevelopment())
            {  
                app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
                {
                     options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                     options.RoutePrefix = string.Empty;
                });
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            /*
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
            
            app.UseMiddleware<AuditMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub").RequireCors("AllowAll");
                endpoints.MapControllers();
            });
        }
    }
}