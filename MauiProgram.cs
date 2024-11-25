using Asis_Batia.Data;
using Asis_Batia.View;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Asis_Batia;

public static class MauiProgram {

    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Condensed", "Regular");

            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        //builder.Services.AddSingleton<IMediaPicker>(MediaPicker.Default);
        //builder.Services.AddTransient<FormuSegAsis>();
       
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<FormuPrinAsis>();
        builder.Services.AddTransient<FormuSegAsis>();

        builder.Services.AddSingleton<DbContext>();

        return builder.Build();
    }
}
