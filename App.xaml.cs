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
    }
}