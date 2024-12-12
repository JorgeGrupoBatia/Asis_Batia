using Asis_Batia.Data;
using Asis_Batia.Helpers;
using Asis_Batia.Model;
using Asis_Batia.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Asis_Batia.ViewModel;

public partial class FormuPrinAsisViewModel : ViewModelBase {

    [ObservableProperty]
    string _siguienteNomenclatura = "";

    [ObservableProperty]
    ObservableCollection<MovimientoModel> _movimientoList;

    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [ObservableProperty]
    bool _isLoading;

    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [ObservableProperty]
    bool _isRefreshing;

    [ObservableProperty]
    string _currentDate = DateTime.Now.ToString("dddd d \\de MMMM");

    [ObservableProperty]
    bool _showConnectivityMsg;

    DbContext _dbContext;

    public FormuPrinAsisViewModel(DbContext dbContext) {
        _dbContext = dbContext;
    }

    [RelayCommand]
    async Task Refresh() {
        await InitMovimientoList();
        IsRefreshing = false;
    }

    [RelayCommand]
    public async Task InitMovimientoList() {
        if(Utils.IsConnectedInternet()) {   
            string url = $"{Constants.API_MOVIMIENTOS_BIOMETA}?idempleado={UserSession.IdEmpleado}";
            MovimientoList = await _httpHelper.GetAsync<ObservableCollection<MovimientoModel>>(url);

            if(MovimientoList is null) {
                await App.Current.MainPage.DisplayAlert("", "Error al obtener los datos, refresque la pantalla", "Ok");
                MovimientoList = new ObservableCollection<MovimientoModel>();
                SiguienteNomenclatura = Constants.SIN_REGISTRO;
                return;
            }
        } else {
            MovimientoModel lastMovimiento = await _dbContext.GetMovimientoAsync(UserSession.IdEmpleado);

            if(lastMovimiento is not null) {
                MovimientoList = new ObservableCollection<MovimientoModel>() {
                    lastMovimiento
                };
            } else {
                MovimientoList = new ObservableCollection<MovimientoModel>();
                if(!UserSession.EsEmpleadoAeropuerto) {
                    SiguienteNomenclatura = Constants.SIN_REGISTRO;
                    return;
                }
            }
        }

        if(MovimientoList.Count > 0) {
            SiguienteNomenclatura = Constants.GetNextRegister(MovimientoList[MovimientoList.Count - 1]);
        } else {
            SiguienteNomenclatura = Constants.A;
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecuteNextPageCommand))]
    private async Task NextPage() {
        DateTime inicioLabores = new DateTime();

        if(MovimientoList.Count > 0 && MovimientoList[0].Movimiento == Constants.A) {
            inicioLabores = MovimientoList[0].Fecha;
        }

        Dictionary<string, object> data = new Dictionary<string, object>{
            { Constants.NOMENCLATURA_KEY, SiguienteNomenclatura },
            { Constants.INICIO_LABORES_KEY, inicioLabores }
        };

        await Shell.Current.GoToAsync(nameof(FormuSegAsis), true, data);
    }

    bool CanExecuteNextPageCommand() {
        return !IsLoading && !IsRefreshing;
    }

    public async Task OnAppearing() {
        IsLoading = true;
        await InitMovimientoList();
        ShowConnectivityMsg = !Utils.IsConnectedInternet();
        IsLoading = false;
    }
}