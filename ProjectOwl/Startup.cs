using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProjectOwl.Data;
using ProjectOwl.Interfaces;
using ProjectOwl.Services;

[assembly: FunctionsStartup(typeof(ProjectOwl.Startup))]
namespace ProjectOwl
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(d => d.UseInMemoryDatabase("AudioDb"));

            builder.Services
                .AddScoped<IBlobStorageService, BlobStorageService>()
                .AddScoped<IAudioService, AudioService>()
                .AddScoped<ISpeechService, SpeechService>()
                .AddScoped<ITextAnalyticsService, TextAnalyticsService>();

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
        }
    }
}
