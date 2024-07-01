using Asis_Batia.Helpers;
using Asis_Batia.Model;
using Asis_Batia.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Asis_Batia.ViewModel;

public partial class FormSegAsisViewModel : ViewModelBase, IQueryAttributable {

    int _idPeriodo, _count;
    public string _selectionRadio, _tipo, _lat, _lng, _localPhotoPath, _dbPhotoPath, _dbFilePathList;
    List<string> _localFilePathList;

    [ObservableProperty]
    bool _isBusy;

    [ObservableProperty]
    string _respuestaTxt;

    [ObservableProperty]
    bool _isEnabled;

    [ObservableProperty]
    bool _isLoading;

    public FormSegAsisViewModel() {
        InitValues();
        _localFilePathList = new List<string>();
    }

    async void InitValues() {
        await GetPeriodo();
    }

    [RelayCommand]
    async Task Validate() {
        if(_selectionRadio == null) {
            await App.Current.MainPage.DisplayAlert("Mensaje", "Seleccione una opción de envío", "Cerrar");
            return;
        }

        await Register();
    }

    async Task Register() {
        try {
            _count = 0;
            IsEnabled = false;
            IsBusy = true;
            IsLoading = true;

            Location currentLocation = new Location();

            if(_selectionRadio == "A") {

                currentLocation = await LocationService.GetCurrentLocation();

                if(currentLocation == null) {
                    IsEnabled = true;
                    IsBusy = false;
                    IsLoading = false;
                    await App.Current.MainPage.DisplayAlert("Mensaje", LocationService.Message, "Cerrar");
                    return;
                }

                CultureInfo culture = new CultureInfo("es-MX");
                Location inmuebleLocation = new Location(double.Parse(_lat, culture), double.Parse(_lng, culture));

                double distanceKm = LocationService.CalcularDistancia(currentLocation, inmuebleLocation);
                if(distanceKm > .400) {
                    if(_count == 0) {

                        _count++;
                        var result = await App.Current.MainPage.DisplayAlert("Acción no permitida", "Parece que estas lejos de tu servicio, ¿Deseas registrarte en otro servicio?", "Si", "No");
                        if(result) {
                            var data = new Dictionary<string, object>{
                                {"Lat", _lat},
                                {"Lng", _lng}
                            };
                            await Shell.Current.GoToAsync(nameof(SelectInmueble), true, data);

                        } else {
                            _count = 0;
                            IsBusy = false;
                            IsEnabled = true;
                            IsLoading = false;
                            return;
                        }
                        IsBusy = false;
                        IsEnabled = true;
                        IsLoading = false;
                        return;
                    }
                }
            }

            if(!await SendFiles()) {
                IsEnabled = true;
                IsBusy = false;
                IsLoading = false;
                await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al enviar los archivos", "Cerrar");
                return;
            }

            RegistroModel registroModel = new RegistroModel {
                Adjuntos = _dbFilePathList == null ? "" : _dbFilePathList,
                Anio = DateTime.Today.Year,
                Confirma = "BIOMETA",
                Cubierto = 0,
                Fecha = DateTime.Now,
                Idempleado = UserSession.IdEmpleado,
                Idinmueble = UserSession.IdInmueble,
                Idperiodo = _idPeriodo,
                Latitud = currentLocation.Latitude.ToString(),
                Longitud = currentLocation.Longitude.ToString(),
                Movimiento = _selectionRadio,
                RespuestaTexto = RespuestaTxt == null ? "" : RespuestaTxt,
                Tipo = _tipo,
                Foto = _dbPhotoPath == null ? "" : _dbPhotoPath,
            };

            int resp = await _httpHelper.PostBodyAsync<RegistroModel, int>(Constants.API_REGISTRO_BIOMETA, registroModel);
            
            IsBusy = false;
            IsEnabled = true;
            IsLoading = false;

            if(resp == 0) {
                await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al registrar los datos", "Ok");
                return;
            }
            
            await MauiPopup.PopupAction.DisplayPopup(new RegExitoso());
        } catch(Exception ) {
            IsBusy = false;
            IsEnabled = false;
            IsLoading = false;
            await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al registrar los datos", "Cerrar");
        }
    }

    async Task GetPeriodo() {
        IsLoading = true;
        string url = $"{Constants.API_PERIODO_NOMINA}?Idempleado={UserSession.IdEmpleado}";
        var periodoEmpleado = await _httpHelper.GetAsync<ObservableCollection<PeriodoNominaModel.PeriodoClient>>(url);
        _idPeriodo = periodoEmpleado[0].id_periodo;
        _tipo = periodoEmpleado[0].descripcion;
        IsLoading = false;
    }

    [RelayCommand]
    private async Task LoadFile() {
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
    }

    [RelayCommand]
    private async Task Photo() {
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
        _lat = (string)query["Lat"];
        _lng = (string)query["Lng"];
    }
}