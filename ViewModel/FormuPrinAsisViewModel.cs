using Asis_Batia.Helpers;
using Asis_Batia.Model;
using Asis_Batia.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Asis_Batia.ViewModel;

public partial class FormuPrinAsisViewModel : ViewModelBase {

    [ObservableProperty]
    ObservableCollection<MovimientoModel> _movimientoList;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    bool _showButton;

    async Task InitMovimientoList() {
        IsLoading = true;
        string url = $"{Constants.API_MOVIMIENTOS_BIOMETA}?idempleado={UserSession.IdEmpleado}";
        MovimientoList = await _httpHelper.GetAsync<ObservableCollection<MovimientoModel>>(url);

        if(MovimientoList is null) {
            await App.Current.MainPage.DisplayAlert("", "Error al obtener los datos, refresque la pantalla", "Ok");
            MovimientoList = new ObservableCollection<MovimientoModel>();
        }

        EvaluarShowButton();

        IsLoading = false;
    }

    [RelayCommand]
    private async Task NextPage() {
        int size = MovimientoList.Count;
        string nomenclatura;
        switch(size) {
            case 0:
                nomenclatura = "A";
                break;
            case 1:
                nomenclatura = "A2";
                break;
            case 2:
                nomenclatura = "A3";
                break;
            case 3:
                nomenclatura = "A4";
                break;
            default:
                nomenclatura = "";
                break;
        }

        Dictionary<string, object> data = new Dictionary<string, object>{
            { Constants.DATA_KEY, nomenclatura }
        };

        await Shell.Current.GoToAsync(nameof(FormuSegAsis), true, data);
    }

    void EvaluarShowButton() {
        try {
            if(MovimientoList.Count < 4) {
                if(MovimientoList.Count > 0) {
                    foreach(MovimientoModel movimiento in MovimientoList) {
                        if(movimiento.Movimiento.Equals("N")) {
                            ShowButton = false;
                            return;
                        }
                    }
                }
                ShowButton = true;
            } else {
                ShowButton = false;
            }
        } catch(Exception) {
            ShowButton = true;
        }
    }

    public async Task OnAppearing() {
        await InitMovimientoList();
    }
}
