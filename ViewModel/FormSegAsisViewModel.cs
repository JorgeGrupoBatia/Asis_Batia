﻿using Asis_Batia.Helpers;
using Asis_Batia.Model;
using Asis_Batia.Popups;
using Asis_Batia.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;

namespace Asis_Batia.ViewModel;

public partial class FormSegAsisViewModel : ViewModelBase, IQueryAttributable {

    public string _selectionRadio, _localPhotoPath, _dbPhotoPath, _dbFilePathList;
    List<string> _localFilePathList = new List<string>();
    Location _currentLocation = new Location();
    bool _getLocation;

    [NotifyCanExecuteChangedFor(nameof(PhotoCommand), nameof(LoadFileCommand))]
    [ObservableProperty]
    bool _isBusy;

    [ObservableProperty]
    string _respuestaTxt;

    [ObservableProperty]
    string _tipoRegistro;

    [ObservableProperty]
    string _nomenclatura = string.Empty;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    [RelayCommand]
    async Task Register() {
        if(_selectionRadio == Nomenclatura) {
            if(await ValidateLocation()) {
                await SendData();
            }
            return;
        }

        await SendData();
    }

    async Task<bool> ValidateLocation() {
        TextLoading = "Obteniendo ubicación ...";
        IsLoading = true;
        IsBusy = true;

        if(!_getLocation) {
            _currentLocation = await LocationService.GetCurrentLocation();
            _getLocation = true;
        }

        if(_currentLocation == null) {
            TextLoading = "";
            IsLoading = false;
            IsBusy = false;
            await App.Current.MainPage.DisplayAlert("Mensaje", LocationService.Message, "Cerrar");
            return false;
        }

        CultureInfo culture = new CultureInfo("es-MX");
        Location inmuebleLocation = new Location();
        try {
            inmuebleLocation = new Location(double.Parse(UserSession.LatitudeInmueble, culture), double.Parse(UserSession.LongitudInmueble, culture));
        } catch(Exception) {
            TextLoading = "";
            IsLoading = false;
            IsBusy = false;

            await App.Current.MainPage.DisplayAlert("Acción no permitida", "Las coordenadas de su servicio no están registradas. \nFavor de revisarlo con su jefe inmediato.", "Aceptar");
            
            return false;
        }

        double distanceKm = LocationService.CalcularDistancia(_currentLocation, inmuebleLocation);
        if(distanceKm > .400) {
            TextLoading = "";
            IsLoading = false;
            IsBusy = false;

            bool result = await App.Current.MainPage.DisplayAlert("Acción no permitida", "Parece que está lejos de su servicio. \n¿Desea registrarse en otro servicio?", "Sí", "No");
            if(result) {
                bool respuesta = await MauiPopup.PopupAction.DisplayPopup<bool>(new SelectInmueble(_currentLocation));
                if(respuesta) {
                    await SendData();
                }

            }
            return false;
        }

        TextLoading = "";
        IsLoading = false;
        IsBusy = false;
        return true;
    }

    async Task SendData() {
        TextLoading = "Enviando registro ...";
        IsLoading = true;
        IsBusy = true;

        if(!await SendFiles()) {
            TextLoading = "";
            IsLoading = false;
            IsBusy = false;
            await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al enviar los archivos", "Cerrar");
            return;
        }

        RegistroModel registroModel = new RegistroModel {
            Adjuntos = _dbFilePathList == null ? "" : _dbFilePathList,
            Confirma = "BIOMETA",
            Cubierto = 0,
            Idempleado = UserSession.IdEmpleado,
            Latitud = _currentLocation.Latitude.ToString(),
            Longitud = _currentLocation.Longitude.ToString(),
            Movimiento = _selectionRadio,
            RespuestaTexto = RespuestaTxt == null ? "" : RespuestaTxt,
            Foto = _dbPhotoPath == null ? "" : _dbPhotoPath,
        };

        int resp = await _httpHelper.PostBodyAsync<RegistroModel, int>(Constants.API_REGISTRO_BIOMETA, registroModel);

        if(resp == 0) {
            TextLoading = "";
            IsLoading = false;
            IsBusy = false;
            await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al registrar los datos", "Ok");
            return;
        }

        TextLoading = "";
        IsLoading = false;
        IsBusy = false;
        await Shell.Current.GoToAsync("..");
        await MauiPopup.PopupAction.DisplayPopup(new RegExitoso());
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task LoadFile() {
        IsBusy = true;
        try {
            var fileResultList = await FilePicker.Default.PickMultipleAsync(GetPickOptions());
            if(fileResultList is not null && fileResultList.Count() > 0) {
                foreach(FileResult fileResult in fileResultList) {
                    string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileResult.FileName);
                    using(Stream stream = await fileResult.OpenReadAsync()) {
                        using FileStream fileStream = File.OpenWrite(localFilePath);
                        await stream.CopyToAsync(fileStream);
                    }
                    _localFilePathList.Add(localFilePath);
                }
            }
        } catch(Exception) {
            await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al seleccionar archivos", "Cerrar");
        }
        IsBusy = false;
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task Photo() {
        IsBusy = true;
        try {
            if(MediaPicker.IsCaptureSupported) {
                FileResult fileResult = await MediaPicker.CapturePhotoAsync();
                if(fileResult != null) {
                    string localPhotoPath = Path.Combine(FileSystem.CacheDirectory, fileResult.FileName);
                    using(Stream photoStream = await fileResult.OpenReadAsync()) {
                        using FileStream fileStream = File.OpenWrite(localPhotoPath);
                        await photoStream.CopyToAsync(fileStream);
                    }
                    _localPhotoPath = localPhotoPath;
                }
            }
        } catch(Exception) {
            await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al capturar la fotografía", "Cerrar");
        }
        IsBusy = false;
    }

    public async Task<bool> SendFiles() {
        if(!string.IsNullOrWhiteSpace(_localPhotoPath)) {
            _localFilePathList.Add(_localPhotoPath);
        }

        if(_localFilePathList.Count > 0) {
            string dbPaths = await UploadFiles();

            if(string.IsNullOrWhiteSpace(dbPaths)) {
                return false;
            }

            string[] paths = dbPaths.Split("|");

            foreach(string path in paths) {
                if(path.Contains(".pdf")) {
                    _dbFilePathList += $"{path}|";
                } else if(path.Contains(".jpg") || path.Contains(".jpeg") || path.Contains(".png")) {
                    _dbPhotoPath = path;
                }
            }
            _dbFilePathList = _dbFilePathList.TrimEnd('|');
            return true;
        }

        return true;
    }

    public async Task<string> UploadFiles() {
        var formData = new MultipartFormDataContent();

        foreach(string localFilePath in _localFilePathList) {
            StreamContent streamContent = new StreamContent(File.OpenRead(localFilePath));
            formData.Add(streamContent, "files", Path.GetFileName(localFilePath));
        }

        string filePathListResponse = await _httpHelper.PostMultipartAsync<string>(Constants.API_ENVIO_ARCHIVOS, formData);

        if(string.IsNullOrWhiteSpace(filePathListResponse)) {
            return null;
        }

        return filePathListResponse;
    }

    PickOptions GetPickOptions() {
        List<string> pickerOptionsAndroid = new List<string> { /*"image/jpeg", "image/png",*/ "application/pdf" };
        List<string> pickerOptionsIOS = new List<string> {/* "public.image",*/ "com.adobe.pdf" };

        FilePickerFileType filePickerFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> {
            {DevicePlatform.Android, pickerOptionsAndroid},
            {DevicePlatform.iOS, pickerOptionsIOS}
        });

        return new PickOptions() {
            PickerTitle = "Seleccione archivos",
            FileTypes = filePickerFileType
        };
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        try {
            Nomenclatura = (string)query[Constants.NOMENCLATURA_KEY];
            TipoRegistro = MovimientoModel.GetTipoRegistro(Nomenclatura);
            _selectionRadio = Nomenclatura;
        } catch(Exception) { }
    }

    bool CanExecute() {
        return !IsBusy;
    }
}