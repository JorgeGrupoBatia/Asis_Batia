using Asis_Batia.Helpers;
using Asis_Batia.View;
using System.Windows.Input;

namespace Asis_Batia;

public partial class AppShell : Shell {

    public AppShell() {
        InitializeComponent();
        BindingContext = this;

        Routing.RegisterRoute(nameof(FormuSegAsis), typeof(FormuSegAsis));
    }

    public ICommand LogoutCommand => new Command(async () => {
        App.Current.MainPage = new MainPage();
        UserSession.ClearSession();
    });
}
