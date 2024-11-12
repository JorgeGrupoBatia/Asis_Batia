using Asis_Batia.Helpers;
using Asis_Batia.View;
using Plugin.Fingerprint;
using System.Windows.Input;

namespace Asis_Batia;

public partial class AppShell : Shell {

    public ICommand LogoutCommand => new Command(() => {
        App.Current.MainPage = new MainPage();
        UserSession.ClearSession();
    });

    public bool ShowConfiguracion { get; set; }

    public AppShell() {
        InitializeComponent();
        InitValues();
        BindingContext = this;
        Routing.RegisterRoute(nameof(FormuSegAsis), typeof(FormuSegAsis));
    }

    async void InitValues() {
        bool isAvailableBiometric = await CrossFingerprint.Current.IsAvailableAsync();
        ShowConfiguracion = isAvailableBiometric && !UserSession.EsEmpleadoAeropuerto;
    }
}
