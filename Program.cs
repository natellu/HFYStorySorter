
namespace HFYStorySorter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddHttpClient();
            builder.Services.AddDbContext<AppDbContenxt>(
                options => options.UseSqlite("Data Source=hfydata.db"));

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
