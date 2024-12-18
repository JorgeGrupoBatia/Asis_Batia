using Asis_Batia.Data;
using Asis_Batia.Helpers;
using Asis_Batia.Model;
using Asis_Batia.Model.Jornaleros;
using Asis_Batia.Popups;
using Asis_Batia.View;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Asis_Batia.ViewModel.Jornaleros;

public partial class JornalerosViewModel : ViewModelBase {

    [ObservableProperty]
    ObservableCollection<JornaleroModel> _jornalerosList;

    [ObservableProperty]
    JornaleroModel _selectedJornalero;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    [ObservableProperty]
    ObservableCollection<ClientModel.Client> _clientList;

    [ObservableProperty]
    ObservableCollection<InmuebleByIdClienteModel.InmuebleModel> _inmuebleList;

    [ObservableProperty]
    ClientModel.Client _selectedClient;

    [ObservableProperty]
    InmuebleByIdClienteModel.InmuebleModel _selectedInmueble;

    [ObservableProperty]
    ObservableCollection<CoberturaModel> _coberturaList;

    [ObservableProperty]
    CoberturaModel _selectedCobertura;

    [ObservableProperty]
    ObservableCollection<EventoModel> _eventoList;

    [ObservableProperty]
    EventoModel _selectedEvento;

    [ObservableProperty]
    object _selectedTipoAsistencia;

    string _urlBase = "https://f9b5-201-148-79-142.ngrok-free.app/api/";
    HttpClient _cli;

    public JornalerosViewModel() {
        _cli = new HttpClient();
        InitValues();
        SelectedTipoAsistencia = "1";
    }

    [RelayCommand]
    async Task SelectClient() {
        if(SelectedClient is null) {
            return;
        }

        if(InmuebleList != null) {
            InmuebleList.Clear();
        }

        IsLoading = true;

        string url = $"{Constants.API_INMUEBLES}?idcliente={SelectedClient.idCliente}";
        InmuebleList = await _httpHelper.GetAsync<ObservableCollection<InmuebleByIdClienteModel.InmuebleModel>>(url);

        IsLoading = false;
    }

    [RelayCommand]
    async Task SelectInmueble() {
        if(SelectedInmueble is null) {
            return;
        }

        if(CoberturaList != null) {
            CoberturaList.Clear();
        }

        if(EventoList != null) {
            EventoList.Clear();
        }

        IsLoading = true;

        ObservableCollection<CoberturaModel> coberturasFVN = new ObservableCollection<CoberturaModel>();
        string urlCoberturas = $"{_urlBase}Coberturas?inmueble={SelectedInmueble.id_inmueble}";
        HttpResponseMessage result = await _cli.GetAsync(urlCoberturas);
        if(result.IsSuccessStatusCode) {
            coberturasFVN = JsonConvert.DeserializeObject<ObservableCollection<CoberturaModel>>(result.Content.ReadAsStringAsync().Result);
        }

        ObservableCollection<CoberturaModel> coberturasVacantes = new ObservableCollection<CoberturaModel>();
        //string urlVacantes = $"{_urlBase}CoberturaVacantes?inmueble={SelectedInmueble.id_inmueble}";
        //HttpResponseMessage resultVacantes = await _cli.GetAsync(urlVacantes);
        //if(resultVacantes.IsSuccessStatusCode) {
        //    coberturasVacantes = JsonConvert.DeserializeObject<ObservableCollection<CoberturaModel>>(resultVacantes.Content.ReadAsStringAsync().Result);
        //}
        if(coberturasFVN.Count > 0) {
            coberturasVacantes = new ObservableCollection<CoberturaModel> {
                new CoberturaModel {
                    IdVacante = 1234,
                    IdTurno = 1,
                    Turno = "MATUTINO"
                },
                new CoberturaModel {
                    IdVacante = 9999,
                    IdTurno = 2,
                    Turno = "VESPERTINO"
                }
            };
        }
        CoberturaList = coberturasFVN.Union<CoberturaModel>(coberturasVacantes).ToObservableCollection();

        string urlEventos = $"{_urlBase}EventosJornaleros?idcliente={SelectedClient.idCliente}&idinmueble={SelectedInmueble.id_inmueble}";
        HttpResponseMessage res = await _cli.GetAsync(urlEventos);

        if(res.IsSuccessStatusCode) {
            EventoList = JsonConvert.DeserializeObject<ObservableCollection<EventoModel>>(res.Content.ReadAsStringAsync().Result);
        }

        IsLoading = false;
    }

    [RelayCommand]
    async void Register() {
        if(SelectedJornalero is null) {
            await App.Current.MainPage.DisplayAlert(Constants.ERROR, "Seleccione Jornalero", Constants.ACEPTAR);
            return;
        }

        if(SelectedClient is null) {
            await App.Current.MainPage.DisplayAlert(Constants.ERROR, "Seleccione Cliente", Constants.ACEPTAR);
            return;
        }

        if(SelectedInmueble is null) {
            await App.Current.MainPage.DisplayAlert(Constants.ERROR, "Seleccione Inmueble", Constants.ACEPTAR);
            return;
        }

        bool esCobertura = SelectedTipoAsistencia.Equals("1");
        bool esEvento = SelectedTipoAsistencia.Equals("2");

        if(esCobertura && SelectedCobertura is null) {
            await App.Current.MainPage.DisplayAlert(Constants.ERROR, "Seleccione Cobertura", Constants.ACEPTAR);
            return;
        }

        if(esEvento && SelectedEvento is null) {
            await App.Current.MainPage.DisplayAlert(Constants.ERROR, "Seleccione Evento", Constants.ACEPTAR);
            return;
        }

        IsLoading = true;
        TextLoading = Constants.ENVIANDO_REGISTROS;

        bool esVacante = esCobertura && SelectedCobertura.IdVacante != 0;

        JornaleroAsistenciaModel registro = new JornaleroAsistenciaModel {
            IdPeriodo = SelectedCobertura is null ? 0 : SelectedCobertura.IdPeriodo,
            Tipo = SelectedCobertura is null ? "" : SelectedCobertura.Tipo,
            Anio = SelectedCobertura is null ? 0 : SelectedCobertura.Anio,
            IdCliente = SelectedClient.idCliente,
            IdInmueble = SelectedInmueble.id_inmueble,
            IdJornalero = SelectedJornalero.IdJornalero,
            IdTurno = SelectedCobertura is null ? 0 : SelectedCobertura.IdTurno,
            Importe = 0,
            TipoAsistencia = esCobertura ? 1 : 2,
            TipoMovimiento = esEvento ? "A" : esVacante ? "L" : SelectedCobertura.Movimiento,
            IdVacante = esVacante ? SelectedCobertura.IdVacante : 0,
            IdEvento = esEvento ? SelectedEvento.IdEvento : 0,
            IdEmpleado = !esEvento && !esVacante ? SelectedCobertura.IdEmpleado : 0
        };

        string url = $"{_urlBase}RegistroJornalerosAsistencia";
        int res = await _httpHelper.PostBodyAsync<JornaleroAsistenciaModel, int>(url, registro);

        if(res > 0) {
            LimpiarValores();
            await App.Current.MainPage.Navigation.PopAsync();
            await MauiPopup.PopupAction.DisplayPopup(new GenericPopup("¡ Registro exitoso !"));
        } else {
            await MauiPopup.PopupAction.DisplayPopup(new GenericPopup("Error al realizar el registro.\nVuelva a intentarlo.", imageUrl: "cerrarsecion"));
        }

        IsLoading = false;
        TextLoading = string.Empty;
    }

    [RelayCommand]
    void Checked() {
        SelectedCobertura = null;
        SelectedEvento = null;
    }

    async void InitValues() {
        TextLoading = "Cargando datos ...";
        IsLoading = true;

        string url = $"{_urlBase}Jornaleros";
        HttpResponseMessage result = await _cli.GetAsync(url);

        if(result.IsSuccessStatusCode) {
            JornalerosList = JsonConvert.DeserializeObject<ObservableCollection<JornaleroModel>>(result.Content.ReadAsStringAsync().Result);
        }
        ClientList = await _httpHelper.GetAsync<ObservableCollection<ClientModel.Client>>(Constants.API_CLIENTE);

        TextLoading = "";
        IsLoading = false;
    }

    void LimpiarValores() {
        SelectedJornalero = null;
        SelectedClient = null;
        SelectedInmueble = null;
        SelectedCobertura = null;
        SelectedEvento = null;
    }
}