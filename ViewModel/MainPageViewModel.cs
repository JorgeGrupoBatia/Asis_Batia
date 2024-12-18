﻿using Asis_Batia.Helpers;
using Asis_Batia.Model;
using Asis_Batia.Popups;
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

            bool isAvailableBiometric = await CrossFingerprint.Current.IsAvailableAsync();

            if(isAvailableBiometric && !UserSession.IsBiometricsActivated && !UserSession.EsEmpleadoAeropuerto) { 
                await MauiPopup.PopupAction.DisplayPopup(new BiometricsPopup());
            }
        } else {
            await App.Current.MainPage.DisplayAlert("Error", "No se encontró ningún empleado", "Cerrar");
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