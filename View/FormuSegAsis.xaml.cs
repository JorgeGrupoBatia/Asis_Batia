using Asis_Batia.Data;
using Asis_Batia.ViewModel;

namespace Asis_Batia.View;

public partial class FormuSegAsis : ContentPage {

    FormSegAsisViewModel _viewModel;

    public FormuSegAsis(DbContext dbContext) {
        InitializeComponent();
        _viewModel = new FormSegAsisViewModel(dbContext);
        BindingContext = _viewModel;
    }

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e) {
        RadioButton radio = sender as RadioButton;
        if(radio.IsChecked) {
            _viewModel._selectionRadio = radio.Value.ToString();
        }
    }

    protected async override void OnAppearing() {
        base.OnAppearing();

        PermissionStatus cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();

        if(cameraStatus != PermissionStatus.Granted) {

            cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
            if(cameraStatus != PermissionStatus.Granted) {
                await DisplayAlert("Permisos necesarios", "Los permisos de cámara son obligatorios para continuar.", "OK");
            }
        }

        PermissionStatus locationStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if(locationStatus != PermissionStatus.Granted) {
            locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if(locationStatus != PermissionStatus.Granted) {
                await DisplayAlert("Permisos necesarios", "Los permisos de ubicación son obligatorios para continuar.", "OK");
            }
        }

        Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();
        Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
    }

    private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e) {
        _viewModel.ShowConnectivityError = e.NetworkAccess != NetworkAccess.Internet;
    }
}