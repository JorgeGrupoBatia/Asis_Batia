using Asis_Batia.Helpers;
using Asis_Batia.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace Asis_Batia.ViewModel;

public partial class SelectInmuebleViewModel : ViewModelBase {

    [ObservableProperty]
    ObservableCollection<ClientModel.Client> _clients;

    [ObservableProperty]
    ObservableCollection<InmuebleByIdClienteModel.InmuebleModel> _inmueble;

    private ClientModel.Client _idClientSelected;

    public ClientModel.Client IdClientSelected {
        get { return _idClientSelected; }
        set {

            if(_idClientSelected != value) {
                _idClientSelected = value;

                OnPropertyChanged();

                GetInmuebleByIdClient(_idClientSelected.idCliente);
            }
        }
    }

    private InmuebleByIdClienteModel.InmuebleModel _idInmubleSelected;
    public InmuebleByIdClienteModel.InmuebleModel IdInmubleSelected {
        get { return _idInmubleSelected; }
        set {
            if(_idInmubleSelected != value && value != null) {
                _idInmubleSelected = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand NextPageCommand { get; set; }

    public SelectInmuebleViewModel() {
        NextPageCommand = new Command(async () => await NextPage());
        GetClients();
    }

    private async Task GetClients() {
        Clients = await _httpHelper.GetAsync<ObservableCollection<ClientModel.Client>>(Constants.API_CLIENTE);
    }

    private async Task GetInmuebleByIdClient(int idCliente) {
        if(Inmueble != null)
            Inmueble.Clear();

        string url = $"{Constants.API_INMUEBLES}?idcliente={idCliente}";

        Inmueble = await _httpHelper.GetAsync<ObservableCollection<InmuebleByIdClienteModel.InmuebleModel>>(url);
    }

    private async Task NextPage() {
        try {
            if(await CheckLocation()) {
                var data = new Dictionary<string, object>{
                    {"Lat", IdInmubleSelected.latitud},
                    {"Lng", IdInmubleSelected.longitud},
                };
                await Shell.Current.GoToAsync("//FormSeg", true, data);
            }
        } catch(Exception) {
            await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al obtener los datos, verifique que todos los campos estén seleccionados o contacte al administrador", "Cerrar");
            return;
        }
    }

    private async Task<bool> CheckLocation() {
        CultureInfo culture = new CultureInfo("es-MX");
        Location CurrentLocation = await LocationService.GetCurrentLocation();
        Location TargetDestination = new Location(double.Parse(IdInmubleSelected.latitud, culture), double.Parse(IdInmubleSelected.longitud, culture));
        if(CurrentLocation == null) {
            var message = LocationService.Message;
            await App.Current.MainPage.DisplayAlert("Mensaje", message, "Cerrar");
            return false;
        }
        if(string.IsNullOrEmpty(IdInmubleSelected.latitud) || string.IsNullOrEmpty(IdInmubleSelected.longitud)) {
            await App.Current.MainPage.DisplayAlert("Alerta", "No se encontraron coordenadas del inmueble seleccionado", "Cerrar");
            return false;
        }
        if(Math.Round(LocationService.CalcularDistancia(CurrentLocation, TargetDestination) * 1000, 2) > 60)//COMPROBAMOS QUE LA DISTANCIA NO SEA MAYOR A 100CM QUE EQUIVALE A 1 METRO, SI NECESITAS CAMBIAR LA DISTANCIA A COMPAR DEBES PONER EN CM LA DISTANCIA
        {
            //IsBusy = false;
            await App.Current.MainPage.DisplayAlert("Acción no permitida", "Está muy lejos del área seleccionada", "Cerrar");
            return false;
        }
        return true;
    }
}