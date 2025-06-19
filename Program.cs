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


            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddHttpClient();
            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseSqlite("Data Source=hfydata.db"));
            builder.Services.AddHostedService<PostFetcherService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            /*app.MapControllers();*/
            app.MapGet("/", () => "HFY Story Sorter API is running!");

            app.Run();
        }
    }
}
