using Asis_Batia.Helpers;
using Asis_Batia.Model;
using Asis_Batia.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Asis_Batia.ViewModel;

public partial class MainPageViewModel : ViewModelBase {

    [ObservableProperty]
    string _idEmpleado;

    public MainPageViewModel() {
        int UID = Convert.ToInt32(Preferences.Get("UserId", 0));
        if(UID > 0) {
            IdEmpleado = UID.ToString();
            GetInfoEmpleado();
        }
    }

    [RelayCommand]
    async Task GetInfoEmpleado() {
        if(string.IsNullOrEmpty(IdEmpleado) || IdEmpleado == "0") {
            await App.Current.MainPage.DisplayAlert("Error", "Ingrese su ID", "Cerrar");
            return;
        }
        HttpHelper httpHelper = new HttpHelper();

        string url = $"{Constants.API_EMPLEADO_APP}?idempleado={IdEmpleado}";
        List<InfoEmpleadoModel> res = await httpHelper.GetAsync<List<InfoEmpleadoModel>>(url);

        if(res is not null && res.Count > 0) {
            UserSession.SetData(res[0]);
            UserSession.IsFirstRun = false;
            await NextPage();
        } else {
            await App.Current.MainPage.DisplayAlert("Error", "No se encontró ningún usuario", "Cerrar");
        }
    }

    private async Task NextPage() {
        if(UserSession.IsFirstRun) {
            await MauiPopup.PopupAction.DisplayPopup<bool>(new PopupRulesPage());
        }
        await Shell.Current.GoToAsync("//FormPrin", true);
    }

    //private async Task Next() {
    //var data = new Dictionary<string, object>
    //    {//en este diccionario estamos estableciendo los valores que nos trajo la consulta y le asignamos una llave a cada valor
    //            {"Empleado", InfoEmpleado[0].empleado },
    //            {"Cliente", InfoEmpleado[0].cliente },
    //            {"PuntoAtencion", InfoEmpleado[0].puntoAtencion },
    //            {"Estado", InfoEmpleado[0].estado },
    //            {"IdCliente", InfoEmpleado[0].idCliente },
    //            {"idEmpleado", InfoEmpleado[0].idEmpleado },
    //            {"idInmueble", InfoEmpleado[0].idInmueble },
    //            {"idEstado", InfoEmpleado[0].idEstado }

    //            // si tefijas tenemos ya todos los datos del usuario y los pasamos como parametro al siguiente form
    //        };
    //await Shell.Current.GoToAsync("//FormPrin", true/*, data*/);// aqui enviamos el diccionario al siguiente formulario


    //}
}