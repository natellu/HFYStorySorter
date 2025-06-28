using HFYStorySorter.Data;
using HFYStorySorter.Logging;
using HFYStorySorter.Services;
using HFYStorySorter.WebUI.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace HFYStorySorter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //wwwroot location
            var options = new WebApplicationOptions
            {
                ContentRootPath = Directory.GetCurrentDirectory(),
                WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "WebUI", "wwwroot")
            };

            var builder = WebApplication.CreateBuilder(options);


            //logging
            builder.Services.AddSingleton<SignalRLogSink>();
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                    .WriteTo.Sink(services.GetRequiredService<SignalRLogSink>());
            });

            //services
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddHttpClient();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=hfydata.db"));

            builder.Services.AddHostedService<PostFetcherService>();
            builder.Services.AddHostedService<StorySorterService>();
            builder.Services.AddSingleton<ServiceStatus>();

            //webui
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();


            builder.Services.AddSignalR();



            builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
                {
                    var secret = builder.Configuration["Jwt:Secret"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret!))
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            var hubContext = app.Services.GetRequiredService<IHubContext<LogHub>>();
            SignalRLogSink.Configure(hubContext);

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseStaticFiles();
            app.UseRouting();

            // webui endpoints
            app.MapRazorPages();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            // signalr endpoint
            app.MapHub<LogHub>("/loghub");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
