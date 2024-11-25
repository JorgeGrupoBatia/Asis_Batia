using Asis_Batia.Data;
using Asis_Batia.ViewModel;

namespace Asis_Batia.View;

public partial class FormuPrinAsis : ContentPage {

    FormuPrinAsisViewModel _viewModel;

    public FormuPrinAsis(DbContext dbContext) {
        InitializeComponent();
        _viewModel = new FormuPrinAsisViewModel(dbContext);
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing() {
        base.OnAppearing();
        await _viewModel.OnAppearing();
        Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();
        Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
    }

    private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e) {
        _viewModel.ShowConnectivityMsg = e.NetworkAccess != NetworkAccess.Internet;
        await _viewModel.InitMovimientoList();
    }
}