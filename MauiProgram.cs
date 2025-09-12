using AppointmentPlanner.Data;
using ExpenseTracker.Service;
using KavaPryct.Services;
using Microsoft.Extensions.Logging;
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


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
