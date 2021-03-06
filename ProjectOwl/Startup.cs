using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProjectOwl.Data;
using ProjectOwl.Interfaces;
using ProjectOwl.Services;
using System;

[assembly: FunctionsStartup(typeof(ProjectOwl.Startup))]
namespace ProjectOwl
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            if(!string.Equals(Environment.GetEnvironmentVariable("Env"), "Local"))
                builder.Services.AddDbContext<ApplicationDbContext>(d => 
                    d.UseSqlServer(Environment.GetEnvironmentVariable("DatabaseConnectionString")));
            else
                builder.Services.AddDbContext<ApplicationDbContext>(d => d.UseInMemoryDatabase("AudioDb"));
            
            builder.Services
                .AddScoped<IBlobStorageService, BlobStorageService>()
                .AddScoped<IAudioService, AudioService>()
                .AddScoped<ISpeechService, SpeechService>();

            builder.Services.AddHttpClient<ITokenService, TokenService>();
            builder.Services.AddHttpClient<ITextAnalyticsService, TextAnalyticsService>();

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
        }
    }
}
