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
        ShowTermsAndConditions();
    }

    [RelayCommand]
    async Task GetInfoEmpleado() {
        if(string.IsNullOrWhiteSpace(IdEmpleado) || IdEmpleado == "0") {
            await App.Current.MainPage.DisplayAlert("Error", "Ingrese su ID de empleado", "Cerrar");
            return;
        }

        string url = $"{Constants.API_EMPLEADO_APP}?idempleado={IdEmpleado}";
        List<InfoEmpleadoModel> res = await _httpHelper.GetAsync<List<InfoEmpleadoModel>>(url);

        if(res is not null && res.Count > 0) {
            UserSession.SetData(res[0]);
            App.Current.MainPage = new AppShell();
        } else {
            await App.Current.MainPage.DisplayAlert("Error", "No se encontró ningún usuario", "Cerrar");
        }
    }

    async void ShowTermsAndConditions() {
        if(UserSession.ShowTermsConditions) {
            await MauiPopup.PopupAction.DisplayPopup<bool>(new PopupRulesPage());
        }
    }
}