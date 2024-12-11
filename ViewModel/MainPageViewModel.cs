using Asis_Batia.Data;
using Asis_Batia.Helpers;
using Asis_Batia.Model;
using Asis_Batia.Popups;
using Asis_Batia.View.Jornaleros;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Fingerprint;
using System.Runtime.InteropServices;

namespace Asis_Batia.ViewModel;

public partial class MainPageViewModel : ViewModelBase {

    [ObservableProperty]
    string _idEmpleado;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    [ObservableProperty]
    bool _showConnectivityMsg;

    [ObservableProperty]
    bool _showWarning;

    DbContext _dbContext;

    public MainPageViewModel() {
        ShowTermsAndConditions();
        ShowConnectivityMsg = !Utils.IsConnectedInternet();
        _dbContext = new DbContext();
    }

    [RelayCommand]
    async Task PrecargarDatos() {
        IsLoading = true;
        TextLoading = Constants.PRECARGANDO_DATOS;

        var empleadosPrecargados = await _httpHelper.GetAsync<List<InfoEmpleadoModel>>(Constants.API_PRECARGAR_EMPLEADOS);
        if(empleadosPrecargados is not null) {
            await _dbContext.DeleteAllEmpleadosAsync();
            await _dbContext.SaveAllEmpleadosAsync(empleadosPrecargados);
            await PrecargarMovimientos(Constants.DATOS_PRECARGADOS_OK, Constants.DATOS_PRECARGADOS_ERROR);
        } else {
            await MauiPopup.PopupAction.DisplayPopup(new GenericPopup(Constants.DATOS_PRECARGADOS_ERROR, imageUrl: "cerrarsecion"));
        }

        TextLoading = "";
        IsLoading = false;
    }

    async Task PrecargarMovimientos(string successMessage, string errorMessage) {
        var movimientosPrecargados = await _httpHelper.GetAsync<List<MovimientoModel>>(Constants.API_PRECARGAR_MOVIMIENTOS);

        if(movimientosPrecargados is not null) {
            await _dbContext.DeleteAllMovimientosAsync();
            await _dbContext.SaveAllMovimientosAsync(movimientosPrecargados);
            await MauiPopup.PopupAction.DisplayPopup(new GenericPopup(successMessage));
        } else {
            await MauiPopup.PopupAction.DisplayPopup(new GenericPopup(errorMessage, imageUrl: "cerrarsecion"));
        }
    }

    [RelayCommand]
    async Task GetInfoEmpleado() {
        if(string.IsNullOrWhiteSpace(IdEmpleado) || IdEmpleado == "0") {
            await App.Current.MainPage.DisplayAlert("Error", "Ingrese su número de Empleado", "Cerrar");
            return;
        }

        if(Utils.IsConnectedInternet()) {
            GetRemoteInfoEmpleado();
        } else {
            await GetLocalInfoEmpleado();
        }
    }

    async void GetRemoteInfoEmpleado() {
        string url = $"{Constants.API_EMPLEADO_APP}?idempleado={IdEmpleado}";

        IsLoading = true;
        List<InfoEmpleadoModel> res = await _httpHelper.GetAsync<List<InfoEmpleadoModel>>(url);
        IsLoading = false;

        if(res is not null && res.Count > 0) {
            UserSession.SetData(res[0]);

            App.Current.MainPage = new AppShell();

            bool isAvailableBiometric = await CrossFingerprint.Current.IsAvailableAsync();

            if(isAvailableBiometric && !UserSession.IsBiometricsActivated && !UserSession.EsEmpleadoAeropuerto) {
                await MauiPopup.PopupAction.DisplayPopup(new BiometricsPopup());
            }
        } else {
            await App.Current.MainPage.DisplayAlert("Error", "No se encontró ningún Empleado", "Cerrar");
        }
    }

    async Task GetLocalInfoEmpleado() {
        IsLoading = true;
        InfoEmpleadoModel empleado = await _dbContext.GetEmpleadoAsync(IdEmpleado);
        IsLoading = false;

        if(empleado is not null) {
            UserSession.SetData(empleado);
            App.Current.MainPage = new AppShell();
        } else {
            await App.Current.MainPage.DisplayAlert("Error", "No se encontró ningún Empleado", "Cerrar");
        }
    }

    [RelayCommand]
    public async Task EnviarRegistros() {
        List<RegistroModel> listRegistros = await _dbContext.GetAllRegistersAsync();

        if(listRegistros is null || listRegistros.Count == 0) {            
            return;
        }

        IsLoading = true;
        TextLoading = Constants.ENVIANDO_REGISTROS;       

        int res = await _httpHelper.PostBodyAsync<List<RegistroModel>, int>(Constants.API_REGISTRO_MASIVO_BIOMETA, listRegistros);

        if(res == 1) {
            await _dbContext.DeleteAllRegisterAsync();
            ShowWarningChanged();
            await PrecargarMovimientos(Constants.REGISTROS_SINCRONIZADOS_OK, Constants.REGISTROS_SINCRONIZADOS_ERROR_2);
        } else {
            await MauiPopup.PopupAction.DisplayPopup(new GenericPopup(Constants.REGISTROS_SINCRONIZADOS_ERROR_1, imageUrl: "cerrarsecion"));
        }

        IsLoading = false;
        TextLoading = "";
    }

    [RelayCommand]
    async Task ShowJornalerosPage() {
        await App.Current.MainPage.Navigation.PushAsync(new JornalerosPage());
    }

    public async void ShowWarningChanged() {
        List<RegistroModel> listRegistros = await _dbContext.GetAllRegistersAsync();

        if(listRegistros is null || listRegistros.Count == 0) {
            ShowWarning = false;
        } else {
            ShowWarning = true;
        }
    }

    async void ShowTermsAndConditions() {
        if(UserSession.ShowTermsConditions) {
            bool res = await MauiPopup.PopupAction.DisplayPopup<bool>(new PopupRulesPage());
            if(res) {
                UserSession.ShowTermsConditions = false;
            } else {
#if IOS
                exit(0);
#else
                Application.Current.Quit();
#endif
            }
        }
    }

#if IOS
    [DllImport("__Internal", EntryPoint = "exit")]
    public static extern void exit(int status);
#endif
}