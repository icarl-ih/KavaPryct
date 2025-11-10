using AppointmentPlanner.Data;
using ExpenseTracker.Service;
using KavaPryct.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;
using System.Net.Http.Headers;

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
            builder.Services.AddScoped<DedupeRemoteService>(sp =>
            {
                var client = new HttpClient { BaseAddress = new Uri(settings.ParseBaseUrl) };
                client.DefaultRequestHeaders.Add("X-Parse-Application-Id", settings.ApplicationId);
                client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", settings.RestApiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return new DedupeRemoteService(client);
            });

            // (si usas estos, mantén también)
            builder.Services.AddScoped<AppointmentService>();
            builder.Services.AddScoped<ExpenseDataService>();

            // --- Conectividad para CommonService ---
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddHttpClient(); // para IHttpClientFactory

            builder
                        .UseMauiApp<App>()
                        .ConfigureLifecycleEvents(events =>
                        {
#if WINDOWS
                events.AddWindows(windows =>
                {
                    windows.OnWindowCreated((window) =>
                    {
                        window.Title = "KAVA: Agenda Virtual"; // O dejar en blanco ""
                    });
                });
#endif
                        });
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
