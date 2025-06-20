using HFYStorySorter.Data;
using HFYStorySorter.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace HFYStorySorter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) //suppress ef logs
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();



            //wwwroot is in the webui folder
            var options = new WebApplicationOptions
            {
                ContentRootPath = Directory.GetCurrentDirectory(),
                WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "WebUI", "wwwroot")
            };

            var builder = WebApplication.CreateBuilder(options);

            builder.Host.UseSerilog();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddHttpClient();
            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseSqlite("Data Source=hfydata.db"));
            builder.Services.AddHostedService<PostFetcherService>();
            builder.Services.AddHostedService<StorySorterService>();
            builder.Services.AddSingleton<ServiceStatus>();

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.MapRazorPages();
            app.MapBlazorHub();

            app.MapFallbackToPage("/_Host");

            /*app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapGet("/", () => "HFY Story Sorter API is running!");*/

            app.Run();
        }
    }
}
