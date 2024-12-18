﻿using Asis_Batia.View;
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

        //Routing.RegisterRoute("PrinP", typeof(PrincipalPage));
        //Routing.RegisterRoute("MenuP", typeof(MenuPage));
        //Routing.RegisterRoute("MainPage", typeof(MainPage));
        //Routing.RegisterRoute("FormPrin", typeof(FormuPrinAsis));
        //Routing.RegisterRoute("FormSeg", typeof(FormuSegAsis));
        //Routing.RegisterRoute("FormReg", typeof(RegExitoso));
        //Routing.RegisterRoute("RulesPage", typeof(MainPage));


        return builder.Build();
    }
}
