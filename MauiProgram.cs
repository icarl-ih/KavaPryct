using AppointmentPlanner.Data;
using ExpenseTracker.Service;
using KavaPryct.Services;
using Microsoft.Extensions.Logging;using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

using Syncfusion.Blazor;

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
            builder.Services.AddScoped<AppointmentService>();
            builder.Services.AddScoped<ExpenseDataService>();
            builder.Services.AddSingleton<EmpleadoRemoteService>();
            builder.Services.AddSingleton < CitasService > ();
            var settings = new AppSettings(); // usa tu clase/valores

            builder.Services.AddScoped<CitasService>(sp =>
            {
                var client = new HttpClient { BaseAddress = new Uri(settings.ParseBaseUrl) };
                client.DefaultRequestHeaders.Add("X-Parse-Application-Id", settings.ApplicationId);
                client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", settings.RestApiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return new CitasService(client);
            });

            builder.Services.AddScoped<EmpleadoRemoteService>(sp =>
            {
                var client = new HttpClient { BaseAddress = new Uri(settings.ParseBaseUrl) };
                client.DefaultRequestHeaders.Add("X-Parse-Application-Id", settings.ApplicationId);
                client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", settings.RestApiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return new EmpleadoRemoteService(client);
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
