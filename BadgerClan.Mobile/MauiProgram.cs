using Microsoft.Extensions.Logging;
using BadgerClan.Mobile.Services;
namespace BadgerClan.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddScoped<HttpClient>((_)=>new HttpClient() { BaseAddress = new Uri("http://localhost:5291") });
            builder.Services.AddScoped<IAPIService, APIService>();
#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
