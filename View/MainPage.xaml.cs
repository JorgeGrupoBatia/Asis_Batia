using Asis_Batia.ViewModel;

namespace Asis_Batia.View;

public partial class MainPage : ContentPage {

    int _cont = 0;
    MainPageViewModel _viewModel;

    public MainPage() {
        InitializeComponent();
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        Loaded += MainPage_Loaded;
        Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();
        Loaded -= MainPage_Loaded;
        Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
    }

    private void MainPage_Loaded(object sender, EventArgs e) {
        if(_cont < 1) {
            _cont++;
            _viewModel = new MainPageViewModel();
            BindingContext = _viewModel;
        }
    }

    private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e) {
        _viewModel.ShowConnectivityMsg = e.NetworkAccess != NetworkAccess.Internet;

        if(!_viewModel.ShowConnectivityMsg) {
            _viewModel.EnviarRegistrosAsync();
        }
    }
}
