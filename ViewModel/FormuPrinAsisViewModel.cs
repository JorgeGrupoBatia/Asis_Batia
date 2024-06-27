using Asis_Batia.Helpers;
using Asis_Batia.Model;
using Asis_Batia.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Asis_Batia.ViewModel;

public partial class FormuPrinAsisViewModel : ViewModelBase {

    [ObservableProperty]
    ObservableCollection<InmuebleByIdClienteModel.InmuebleModel> _inmuebleList;

    [ObservableProperty]
    string _empleado;

    [ObservableProperty]
    string _cliente;

    [ObservableProperty]
    string _estado;

    [ObservableProperty]
    string _inmueble;

    [ObservableProperty]
    bool _isLoading;

    public FormuPrinAsisViewModel() {
        Empleado = UserSession.Empleado;
        Cliente = UserSession.Cliente;
        Inmueble = UserSession.Inmueble;
        Estado = UserSession.Estado;
    }

    async Task GetInmuebleByIdClient() {
        string url = $"{Constants.API_INMUEBLES}?idcliente={UserSession.IdCliente}";
        InmuebleList = await _httpHelper.GetAsync<ObservableCollection<InmuebleByIdClienteModel.InmuebleModel>>(url); 
    }

    [RelayCommand]
    private async Task NextPage() {
        IsLoading = true;
        await GetInmuebleByIdClient();

        if(InmuebleList is null || InmuebleList.Count == 0) {
            await App.Current.MainPage.DisplayAlert("", "Ocurrió un error al enviar los datos. Intente más tarde", "cerrar");
            IsLoading = false;
            return;
        }

        string latitud = null;
        string longitud = null;

        foreach(var inmueble in InmuebleList) {
            if(inmueble.id_inmueble == UserSession.IdInmueble) {
                latitud = inmueble.latitud;
                longitud = inmueble.longitud;
                break;
            }
        }

        Dictionary<string, object> data = new Dictionary<string, object>{
            {"Lat", latitud},
            {"Lng", longitud}
        };

        await Shell.Current.GoToAsync(nameof(FormuSegAsis), true, data);
        IsLoading = false;
    }
}
