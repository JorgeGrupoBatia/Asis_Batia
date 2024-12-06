using Asis_Batia.Data;
using Asis_Batia.Helpers;
using Asis_Batia.Model;
using Asis_Batia.Model.Jornaleros;
using Asis_Batia.View;
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
    ObservableCollection<RegistroModel> _coberturaList;

    [ObservableProperty]
    RegistroModel _selectedCobertura;

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
        string urlCoberturas = $"{_urlBase}Coberturas?inmueble={SelectedInmueble.id_inmueble}";
        HttpResponseMessage result = await _cli.GetAsync(urlCoberturas);

        if(result.IsSuccessStatusCode) {
            CoberturaList = JsonConvert.DeserializeObject<ObservableCollection<RegistroModel>>(result.Content.ReadAsStringAsync().Result);
        }

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

        int idVacante = 0;

        JornaleroAsistenciaModel registro = new JornaleroAsistenciaModel {
            IdPeriodo = SelectedCobertura is null ? 0 : SelectedCobertura.IdPeriodo,
            Tipo = SelectedCobertura is null ? "" : SelectedCobertura.Tipo,
            Anio = SelectedCobertura is null ? 0 : SelectedCobertura.Anio,
            IdCliente = SelectedClient.idCliente,
            IdInmueble = SelectedInmueble.id_inmueble,
            IdJornalero = SelectedJornalero.IdJornalero,
            TipoAsistencia = int.Parse(SelectedTipoAsistencia.ToString()),
            TipoMovimiento = esEvento ? "A" : SelectedCobertura.Movimiento, // L
            IdVacante = esEvento ? 0 : idVacante,
            IdEvento = esEvento ? SelectedEvento.IdEvento : 0,
            IdEmpleado = esEvento ? 0 : idVacante == 0 ? SelectedCobertura.Idempleado : 0
        };

        Dictionary<string, object> data = new Dictionary<string, object>{
            { "jornaleros key", registro }
        };

        await App.Current.MainPage.Navigation.PushAsync(new FormuSegAsis(new DbContext()));
        //await Shell.Current.GoToAsync(nameof(FormuSegAsis), true, data);
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
}