using AppointmentPlanner.Data;
using ExpenseTracker.Service;
using KavaPryct.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;
using System.Net.Http.Headers;

namespace KavaPryct
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXZedXVTQmRcWExzWEJWYEg=");

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            builder.Services.AddSyncfusionBlazor();
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddScoped<SfDialogService>();
            builder.Services.AddScoped<AppointmentService>();
            builder.Services.AddScoped<ExpenseDataService>();
            builder.Services.AddSingleton<EmpleadoRemoteService>();
            builder.Services.AddSingleton < CitasService > ();
            builder.Services.AddSingleton < PacienteService > ();
            builder.Services.AddSingleton < TransactionService > ();
            var settings = new AppSettings(); // usa tu clase/valores

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
            // Registros necesarios
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddHttpClient(); // Para IHttpClientFactory

            var app = builder.Build();

            // Inicializa CommonService con dependencias de conectividad
            var connectivity = app.Services.GetRequiredService<IConnectivity>();
            var httpFactory = app.Services.GetRequiredService<IHttpClientFactory>();
            CommonService.InitConnectivity(connectivity, httpFactory);

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
