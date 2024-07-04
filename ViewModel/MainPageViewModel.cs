﻿using Asis_Batia.Helpers;
using Asis_Batia.Model;
using Asis_Batia.Popups;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Asis_Batia.ViewModel;

public partial class MainPageViewModel : ViewModelBase {

    [ObservableProperty]
    string _idEmpleado;

    [ObservableProperty]
    bool _isLoading;

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

        IsLoading = true;
        List<InfoEmpleadoModel> res = await _httpHelper.GetAsync<List<InfoEmpleadoModel>>(url);
        IsLoading = false;

        if(res is not null && res.Count > 0) {
            UserSession.SetData(res[0]);
            App.Current.MainPage = new AppShell();
        } else {
            await App.Current.MainPage.DisplayAlert("Error", "No se encontró ningún usuario", "Cerrar");
        }
    }

    async void ShowTermsAndConditions() {
        if(UserSession.ShowTermsConditions) {
            bool res = await MauiPopup.PopupAction.DisplayPopup<bool>(new PopupRulesPage());
            if(res) {
                UserSession.ShowTermsConditions = false;
            } else {
                Application.Current.Quit();
            }
        }
    }
}