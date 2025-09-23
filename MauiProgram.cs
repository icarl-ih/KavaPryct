using AppointmentPlanner.Data;
using ExpenseTracker.Service;
using KavaPryct.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;
using System.Net.Http.Headers;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Networking;     // IConnectivity
using Syncfusion.Blazor;
using KavaPryct;                      // tu namespace de App
using ExpenseTracker.Service;         // CommonService

namespace KavaPryct
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
                "Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXZedXVTQmRcWExzWEJWYEg="
            );

            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // UI / Blazor
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSyncfusionBlazor();
            builder.Services.AddScoped<SfDialogService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // --- Tus servicios de dominio ---
            var settings = new AppSettings(); // tu clase/valores

            builder.Services.AddScoped<CitasService>(sp =>
            {
                var client = new HttpClient { BaseAddress = new Uri(settings.ParseBaseUrl) };
                client.DefaultRequestHeaders.Add("X-Parse-Application-Id", settings.ApplicationId);
                client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", settings.RestApiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return new CitasService(client);
            });

            builder.Services.AddScoped<PacienteService>(sp =>
            {
                var client = new HttpClient { BaseAddress = new Uri(settings.ParseBaseUrl) };
                client.DefaultRequestHeaders.Add("X-Parse-Application-Id", settings.ApplicationId);
                client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", settings.RestApiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return new PacienteService(client);
            });

            builder.Services.AddScoped<EmpleadoRemoteService>(sp =>
            {
                var client = new HttpClient { BaseAddress = new Uri(settings.ParseBaseUrl) };
                client.DefaultRequestHeaders.Add("X-Parse-Application-Id", settings.ApplicationId);
                client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", settings.RestApiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return new EmpleadoRemoteService(client);
            });

            builder.Services.AddScoped<TransactionService>(sp =>
            {
                var client = new HttpClient { BaseAddress = new Uri(settings.ParseBaseUrl) };
                client.DefaultRequestHeaders.Add("X-Parse-Application-Id", settings.ApplicationId);
                client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", settings.RestApiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return new TransactionService(client);
            });
            builder.Services.AddScoped<UpdaterService>(sp =>
            {
                var client = new HttpClient { BaseAddress = new Uri(settings.ParseBaseUrl) };
                client.DefaultRequestHeaders.Add("X-Parse-Application-Id", settings.ApplicationId);
                client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", settings.RestApiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return new UpdaterService(client);
            });

            // (si usas estos, mantén también)
            builder.Services.AddScoped<AppointmentService>();
            builder.Services.AddScoped<ExpenseDataService>();

            // --- Conectividad para CommonService ---
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddHttpClient(); // para IHttpClientFactory

            // IMPORTANTÍSIMO: construir SOLO UNA VEZ
            var app = builder.Build();

            // Inicializa CommonService (internet reachability + eventos)
            var connectivity = app.Services.GetRequiredService<IConnectivity>();
            var httpFactory = app.Services.GetRequiredService<IHttpClientFactory>();
            CommonService.InitConnectivity(connectivity, httpFactory);

            return app;
        }
    }
}
