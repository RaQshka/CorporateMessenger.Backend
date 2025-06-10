/*
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
*/

using Messenger.Persistence;

namespace Messenger.WebApi
{
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            try
            {
                using (var scope = host.Services.CreateScope())
                {
                    //var context = scope.ServiceProvider.GetRequiredService<MessengerDbContext>();
                    await DbInitializer.InitializeAsync(scope.ServiceProvider);
                    
                }

            }
            catch (Exception ex)
            {

            }
            host.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}