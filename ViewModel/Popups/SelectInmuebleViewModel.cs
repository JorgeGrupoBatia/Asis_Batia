using Asis_Batia.Helpers;
using Asis_Batia.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPopup;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Asis_Batia.ViewModel.Popups;

public partial class SelectInmuebleViewModel : ViewModelBase {

    [ObservableProperty]
    ObservableCollection<ClientModel.Client> _clientList;

    [ObservableProperty]
    ObservableCollection<InmuebleByIdClienteModel.InmuebleModel> _inmuebleList;

    [ObservableProperty]
    ClientModel.Client _selectedClient;

    [ObservableProperty]
    InmuebleByIdClienteModel.InmuebleModel _selectedInmueble;

    Location _currentLocation;

    public SelectInmuebleViewModel(Location currentLocation) {
        _currentLocation = currentLocation;
        GetClients();
    }

    [RelayCommand]
    async Task SelectClient() {
        if(SelectedClient is null) {
            return;
        }

        if(InmuebleList != null) {
            InmuebleList.Clear();
        }

        string url = $"{Constants.API_INMUEBLES}?idcliente={SelectedClient.idCliente}";
        InmuebleList = await _httpHelper.GetAsync<ObservableCollection<InmuebleByIdClienteModel.InmuebleModel>>(url);
    }

    [RelayCommand]
    async Task NextPage() {
        try {
            if(await CheckLocation()) {
                await PopupAction.ClosePopup(true);
            }
        } catch(Exception) {
            await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al obtener los datos, verifique que todos los campos estén seleccionados o contacte al administrador", "Cerrar");
        }
    }

    [RelayCommand]
    async void Cancel() {
        await PopupAction.ClosePopup(false);
    }

    async Task<bool> CheckLocation() {
        if(_currentLocation == null) {
            var message = LocationService.Message;
            await App.Current.MainPage.DisplayAlert("Mensaje", message, "Cerrar");
            return false;
        }

        if(string.IsNullOrEmpty(SelectedInmueble.latitud) || string.IsNullOrEmpty(SelectedInmueble.longitud)) {
            await App.Current.MainPage.DisplayAlert("Alerta", "No se encontraron coordenadas del inmueble seleccionado", "Cerrar");
            return false;
        }

        CultureInfo culture = new CultureInfo("es-MX");
        Location inmuebleLocation = new Location(double.Parse(SelectedInmueble.latitud, culture), double.Parse(SelectedInmueble.longitud, culture));
        if(Math.Round(LocationService.CalcularDistancia(_currentLocation, inmuebleLocation) * 1000, 2) > 60) {
            await App.Current.MainPage.DisplayAlert("Acción no permitida", "Está muy lejos del área seleccionada", "Cerrar");
            return false;
        }
        return true;
    }

    async void GetClients() {
        ClientList = await _httpHelper.GetAsync<ObservableCollection<ClientModel.Client>>(Constants.API_CLIENTE);
    }
}