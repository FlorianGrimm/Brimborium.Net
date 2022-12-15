using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Brimborium.LocalObservability.TestWebApplication1 {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddReactiveLogger((reactiveLoggerOptions) => {
                reactiveLoggerOptions.IncludeScopes = true;
            });
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                // options.Cookie.Name = ".TestWebApplication1.Session";
            });

            builder.Services.AddSingleton<Logic>();
            // TODO builder
            builder.Services.AddSingleton<IReactiveLoggerSink, Brimborium.LocalObservability.ReactiveLoggerSinkMatcher>();
            builder.Services.AddSingleton<IMatchingEngine, MatchingEngine>();
            builder.Services.AddSingleton<IncidentProcessingEngine, IncidentProcessingEngine>();
            builder.Services.AddSingleton<IIncidentProcessingEngine1>((sp)=>sp.GetRequiredService<IncidentProcessingEngine>());
            builder.Services.AddSingleton<IIncidentProcessingEngine2>((sp)=>sp.GetRequiredService<IncidentProcessingEngine>());
            builder.Services.AddSingleton<MatchingEngineOptions>((serviceProvider) => {
                var result = new MatchingEngineOptions();
                result.ListMatchingRule.Add(new ForAspNetCore.MatchingRuleForAspNetCore());
                result.ListStateTransition.Add(new ForAspNetCore.StateTransitionForAspNetCore());
                return result;
            });
            builder.Services.AddHostedService<IncidentProcessingEngineHostedService>();
            var app = builder.Build();
            app.Services.UseServiceReactiveLoggerSource();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            app.MapRazorPages();

            app.Run();
        }
    }
}