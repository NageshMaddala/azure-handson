using Azure.Storage.Blobs;
using SampleBlobProject.Services;

namespace SampleBlobProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton(u => new BlobServiceClient(
                builder.Configuration.GetValue<string>("BlobConnection")));

            builder.Services.AddSingleton<IContainerService, ContainerService>();
            builder.Services.AddSingleton<IBlobService, BlobService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}