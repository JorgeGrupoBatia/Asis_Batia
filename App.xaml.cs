using Asis_Batia.Controls;
using Asis_Batia.Helpers;
using Asis_Batia.View;

namespace Asis_Batia;

public partial class App : Application {
    public App() {
        InitializeComponent();

        if(UserSession.IdEmpleado > 0) {
            MainPage = new AppShell();
        } else {
            MainPage = new MainPage();
        }

        CreateControls();
    }

    void CreateControls() {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(TransparentEntry), (handler, view) => {
            if(view is TransparentEntry) {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            }
        });
    }
}