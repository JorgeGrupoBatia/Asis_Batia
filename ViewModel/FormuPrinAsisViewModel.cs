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

    async Task InitMovimientoList() {
        IsLoading = true;
        string url = $"{Constants.API_MOVIMIENTOS_BIOMETA}?idempleado={UserSession.IdEmpleado}";
        MovimientoList = await _httpHelper.GetAsync<ObservableCollection<MovimientoModel>>(url);
        if(MovimientoList is null) {
            await App.Current.MainPage.DisplayAlert("", "Error al obtener los datos, refresque la pantalla", "Ok");
        }
        IsLoading = false;
    }

    [RelayCommand]
    private async Task NextPage() {
        await Shell.Current.GoToAsync(nameof(FormuSegAsis), true);
    }

    public async Task OnAppearing() {
        await InitMovimientoList();
    }
}
