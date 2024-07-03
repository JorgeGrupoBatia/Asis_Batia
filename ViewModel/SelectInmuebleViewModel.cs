using Asis_Batia.Helpers;
using Asis_Batia.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Asis_Batia.ViewModel;

public partial class SelectInmuebleViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    ObservableCollection<ClientModel.Client> _clientList;

    [ObservableProperty]
    ObservableCollection<InmuebleByIdClienteModel.InmuebleModel> _inmuebleList;

    [ObservableProperty]
    ClientModel.Client _selectedClient;

    [ObservableProperty]
    InmuebleByIdClienteModel.InmuebleModel _selectedInmueble;

    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [ObservableProperty]
    bool _isLoading;

    Location _currentLocation;

    public SelectInmuebleViewModel() {
        GetClients();
    }

    [RelayCommand]
    async Task SelectClient() {
        IsLoading = true;
        if(SelectedClient is null) {
            return;
        }

        if(InmuebleList != null) {
            InmuebleList.Clear();
        }

        string url = $"{Constants.API_INMUEBLES}?idcliente={SelectedClient.idCliente}";

        InmuebleList = await _httpHelper.GetAsync<ObservableCollection<InmuebleByIdClienteModel.InmuebleModel>>(url);
        IsLoading = false;
    }

    [RelayCommand(CanExecute = (nameof(CanExecute)))]
    private async Task NextPage() {
        try {
            if(await CheckLocation()) {
                Dictionary<string, object> data = new Dictionary<string, object>{
                    {Constants.SEND_DATA_KEY, true},
                };
                await Shell.Current.GoToAsync("..", true, data);
            }
        } catch(Exception) {
            await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al obtener los datos, verifique que todos los campos estén seleccionados o contacte al administrador", "Cerrar");
        }
    }

    private async Task<bool> CheckLocation() {
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
            //IsBusy = false;
            await App.Current.MainPage.DisplayAlert("Acción no permitida", "Está muy lejos del área seleccionada", "Cerrar");
            return false;
        }
        return true;
    }

    private async Task GetClients() {
        IsLoading = true;
        ClientList = await _httpHelper.GetAsync<ObservableCollection<ClientModel.Client>>(Constants.API_CLIENTE);
        IsLoading = false;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        try {
            _currentLocation = (Location)query[Constants.LOCATION_KEY];
        } catch(Exception) { }
    }

    bool CanExecute() {
        return !IsLoading;
    }
}