using Asis_Batia.Data;
using Asis_Batia.Helpers;
using Asis_Batia.Model;
using Asis_Batia.Popups;
using Asis_Batia.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Asis_Batia.ViewModel;

public partial class FormSegAsisViewModel : ViewModelBase, IQueryAttributable {

    public string _selectionRadio, _dbPhotoPathList, _dbFilePathList;
    Location _currentLocation = new Location();
    bool _getLocation;
    DateTime _iniciolabores;
    DbContext _dbContext;

    [NotifyCanExecuteChangedFor(nameof(RegisterCommand), nameof(PhotoCommand), nameof(LoadFileCommand))]
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

    [ObservableProperty]
    string _localPhotoPath;

    [ObservableProperty]
    string _localFilePath;

    [ObservableProperty]
    string _fileName;

    [ObservableProperty]
    bool _showFile;

    [ObservableProperty]
    bool _showConnectivityError;

    public FormSegAsisViewModel(DbContext dbContext) {
        ShowConnectivityError = !Utils.IsConnectedInternet();
        _dbContext = dbContext;
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    async Task Register() {
        IsBusy = true;

        if(Utils.IsConnectedInternet()) {
            if(!await ValidateBiometricAsync()) {
                await App.Current.MainPage.DisplayAlert(Constants.ERROR, Constants.NO_COINCIDE_BIOMETRIA, Constants.ACEPTAR);
                IsBusy = false;
                return;
            }

            if(UserSession.EsEmpleadoElektra) {
                if(string.IsNullOrWhiteSpace(FileName)) {
                    await App.Current.MainPage.DisplayAlert("", "Ingrese captura de pantalla de \'Proveedores GS\'", Constants.ACEPTAR);
                    IsBusy = false;
                    return;
                }
            }
            ValidateNomenclature();
            IsBusy = false;

        } else {            
            if(UserSession.EsEmpleadoAeropuerto) {
                await SaveLocalData();
            } else {
                await App.Current.MainPage.DisplayAlert(Constants.ERROR, Constants.SIN_CONEXION, Constants.ACEPTAR);
            }
            IsBusy = false;
        }
    }

    async Task<bool> ValidateBiometricAsync() {
        try {
            bool isAvailableBiometric = await CrossFingerprint.Current.IsAvailableAsync();

            if(!isAvailableBiometric || !UserSession.IsBiometricsActivated || UserSession.EsEmpleadoAeropuerto) {
                return true;
            }

            var request = new AuthenticationRequestConfiguration(Constants.INGRESE_DATOS_BIOMETRICOS, Constants.COLOQUE_ROSTRO_HUELLA);
            var result = await CrossFingerprint.Current.AuthenticateAsync(request);

            if(!result.Authenticated) {
                return false;
            }

            return true;

        } catch(Exception ex) {
            return true;
        }
    }

    async void ValidateNomenclature() {
        if(_selectionRadio == Nomenclatura) {
            if(await ValidateLocation()) {
                await SendData();
            }
            return;
        }

        await SendData();
    }

    async Task<bool> ValidateLocation() {
        DateTime now = DateTime.Now;
        TimeSpan timeSpan = now - _iniciolabores;
        Double minutosDiferencia = timeSpan.TotalMinutes;

        if(minutosDiferencia < 60 && !UserSession.EsEmpleadoElektra) {
            await App.Current.MainPage.DisplayAlert(Constants.ERROR, 
                "No puede realizar su registro. \n\nSu registro anterior fue realizado recientemente.", Constants.ACEPTAR);
            await Shell.Current.GoToAsync("..");
            return false;
        }

        if(TipoRegistro.Equals(Constants.FIN_LABORES)) {
            bool resp = await App.Current.MainPage.DisplayAlert(""
                , $"Está a punto de registrar {Constants.FIN_LABORES.ToUpper()}, su {Constants.INICIO_LABORES.ToUpper()} fue el día {_iniciolabores.ToString("dddd dd-MMMM a la(\\s) hh:mm tt")}\n\n¿Desea continuar?"
                , Constants.SI, Constants.NO);
            if(!resp) {
                return false;
            }
        }

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
            _getLocation = false;
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

        double distanciaKm = LocationService.CalcularDistancia(_currentLocation, inmuebleLocation);
        float radioTolerancia = UserSession.EsEmpleadoTerminal1 ? 0.750f : 0.400f;
        if(distanciaKm > radioTolerancia) {
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
            Foto = _dbPhotoPathList == null ? "" : _dbPhotoPathList,
        };

        int resp = await _httpHelper.PostBodyAsync<RegistroModel, int>(Constants.API_REGISTRO_BIOMETA, registroModel);

        if(resp == 0) {
            TextLoading = "";
            IsLoading = false;
            IsBusy = false;
            await App.Current.MainPage.DisplayAlert("Error", "Ocurrió una interrupción en el flujo del envío de datos.\n\nRevise sus registros y de ser necesario vuelva a intentarlo.", "Ok");
            await Shell.Current.GoToAsync("..");
            return;
        }

        TextLoading = "";
        IsLoading = false;
        IsBusy = false;

        if(UserSession.EsEmpleadoAeropuerto) {
            try {
                string url = $"{Constants.API_MOVIMIENTOS_BIOMETA}?idempleado={UserSession.IdEmpleado}";
                var listMovs = await _httpHelper.GetAsync<ObservableCollection<MovimientoModel>>(url);

                var lastMovimiento = listMovs[0];
                lastMovimiento.IdEmpleado = UserSession.IdEmpleado;
                await _dbContext.SaveMovimientoAsync(lastMovimiento);

            } catch { }

            App.Current.MainPage = new MainPage();
            UserSession.ClearSession();
        } else {
            await Shell.Current.GoToAsync("..");
        }       

        await MauiPopup.PopupAction.DisplayPopup(new RegExitoso());
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task LoadFile() {
        IsBusy = true;
        try {
            FileResult fileResult = await FilePicker.Default.PickAsync(GetPickOptions());
            if(fileResult is not null) {
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileResult.FileName);
                using(Stream stream = await fileResult.OpenReadAsync()) {
                    using FileStream fileStream = File.OpenWrite(localFilePath);
                    await stream.CopyToAsync(fileStream);
                }
                LocalFilePath = localFilePath;
                SetFileName();
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
                    LocalPhotoPath = localPhotoPath;
                }
            }
        } catch(Exception) {
            await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al capturar la fotografía", "Cerrar");
        }
        IsBusy = false;
    }

    public async Task<bool> SendFiles() {
        if(string.IsNullOrWhiteSpace(LocalPhotoPath) && string.IsNullOrWhiteSpace(LocalFilePath)) {
            return true;
        }

        string dbPaths = await UploadFiles();

        if(string.IsNullOrWhiteSpace(dbPaths)) {
            return false;
        }

        string[] paths = dbPaths.Split("|");

        foreach(string path in paths) {

            string[] tokens = path.Split('/');

            if(!string.IsNullOrWhiteSpace(LocalPhotoPath)) {
                string[] tokensLocal = LocalPhotoPath.Split('/');
                if(tokens[tokens.Length - 1].Equals(tokensLocal[tokensLocal.Length - 1])) {
                    _dbPhotoPathList = path;
                }
            }

            if(!string.IsNullOrWhiteSpace(LocalFilePath)) {
                string[] tokensLocal = LocalFilePath.Split('/');
                if(tokens[tokens.Length - 1].Equals(tokensLocal[tokensLocal.Length - 1])) {
                    _dbFilePathList = path;
                }
            }
        }

        return true;
    }

    public async Task<string> UploadFiles() {

        var formData = new MultipartFormDataContent();

        if(!string.IsNullOrWhiteSpace(LocalPhotoPath)) {
            byte[] fileBytesArray = File.ReadAllBytes(LocalPhotoPath);
            byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytesArray, 300, 300);
            ByteArrayContent byteArrayContent = new ByteArrayContent(resizedImage);
            formData.Add(byteArrayContent, "files", Path.GetFileName(LocalPhotoPath));
        }

        if(!string.IsNullOrWhiteSpace(LocalFilePath)) {
            byte[] fileBytesArray = File.ReadAllBytes(LocalFilePath);
            string fileName = Path.GetFileName(LocalFilePath);

            if(fileName.EndsWith(".pdf")) {
                ByteArrayContent byteArrayContent = new ByteArrayContent(fileBytesArray);
                formData.Add(byteArrayContent, "files", fileName);
            } else {
                byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytesArray, 300, 300, false);
                ByteArrayContent byteArrayContent = new ByteArrayContent(resizedImage);
                formData.Add(byteArrayContent, "files", fileName);
            }
        }

        string filePathListResponse = await _httpHelper.PostMultipartAsync<string>(Constants.API_ENVIO_ARCHIVOS, formData);

        if(string.IsNullOrWhiteSpace(filePathListResponse)) {
            return null;
        }

        return filePathListResponse;
    }

    PickOptions GetPickOptions() {
        List<string> pickerOptionsAndroid = new List<string> { "image/jpeg", "image/png", "application/pdf" };
        List<string> pickerOptionsIOS = new List<string> { "public.image", "com.adobe.pdf" };

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
            TipoRegistro = Constants.GetRegisterType(Nomenclatura);
            _selectionRadio = Nomenclatura;
            _iniciolabores = (DateTime)query[Constants.INICIO_LABORES_KEY];
        } catch(Exception) { }
    }

    bool CanExecute() {
        return !IsBusy;
    }

    void SetFileName() {
        if(!string.IsNullOrWhiteSpace(LocalFilePath)) {
            string[] tokens = LocalFilePath.Split('/');
            FileName = tokens[tokens.Length - 1];
            ShowFile = FileName.EndsWith(".jpg") || FileName.EndsWith(".jpeg") || FileName.EndsWith(".png");
            return;
        }
        FileName = string.Empty;
        ShowFile = false;
    }

    async Task SaveLocalData() {
        TextLoading = "Guardando registro ...";
        IsLoading = true;
        IsBusy = true;

        RegistroModel registroLocal = new RegistroModel {
            // Adjuntos = _dbFilePathList == null ? "" : _dbFilePathList,
            Confirma = "BIOMETA",
            Cubierto = 0,
            Idempleado = UserSession.IdEmpleado,
            Latitud = "",
            Longitud = "",
            Movimiento = _selectionRadio,
            RespuestaTexto = RespuestaTxt == null ? "" : RespuestaTxt,
            Fecha = DateTime.Now
            //Foto = _dbPhotoPathList == null ? "" : _dbPhotoPathList,
        };

        MovimientoModel movimiento = new MovimientoModel {
            IdEmpleado = registroLocal.Idempleado,
            Fecha = registroLocal.Fecha,
            Movimiento = registroLocal.Movimiento
        };

        await _dbContext.SaveRegisterAsync(registroLocal);

        TextLoading = "";
        IsLoading = false;
        IsBusy = false;

        App.Current.MainPage = new MainPage();
        UserSession.ClearSession();

        try {
            await _dbContext.SaveMovimientoAsync(movimiento);
        } catch { }

        await MauiPopup.PopupAction.DisplayPopup(new RegExitoso());
    }
}