using Asis_Batia.Helpers;
using Asis_Batia.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Asis_Batia.ViewModel;

public partial class FormSegAsisViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    bool _isBusy;

    int count = 0;
    int _idPeriodo;
    string _tipo;
    public string _selectionRadio;

    public int IdCliente { get; set; }
    public string NombreCliente { get; set; }
    public int IdEmpleado { get; set; }
    public int IdInmueble { get; set; }
    public string Lat { get; set; }
    public string Lng { get; set; }
    public string PathPhoto { get; set; }
    public string PathFile { get; set; }

    public string SelectionRadio {
        get { return _selectionRadio; }
        set { _selectionRadio = value; OnPropertyChanged(); }
    }

    private string _respuestaTxt;

    public string RespuestaTxt {
        get { return _respuestaTxt; }
        set { _respuestaTxt = value; OnPropertyChanged(); }
    }

    public byte FileBase64 { get; set; }
    public byte Foto { get; set; }

    private bool _isEnabled;
    private readonly IMediaPicker mediaPicker;

    public bool IsEnabled {
        get { return _isEnabled; }
        set { _isEnabled = value; OnPropertyChanged(); }
    }
    public string UrlFiles { get; set; }

    List<string> archivos = new List<string>();

    public FormSegAsisViewModel(IMediaPicker mediaPicker) {
        this.mediaPicker = mediaPicker;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        Lat = (string)query["Lat"];
        Lng = (string)query["Lng"];
    }

    [RelayCommand]
    private async Task Register() {
        try {
            IsEnabled = false;
            count = 0;
            IsBusy = true;

            if(_selectionRadio == null) {
                await App.Current.MainPage.DisplayAlert("Mensaje", "Seleccione una opción de envío", "Cerrar");
                IsEnabled = true;
                IsBusy = false;
                return;
            }

            CultureInfo culture = new CultureInfo("es-MX");// Establece la cultura adecuada para México
            Location _location = await LocationService.GetCurrentLocation();
            double latitud = double.Parse(Lat, culture);
            double longitud = double.Parse(Lng, culture);
            Location TargetDestination = new Location(latitud, longitud);// new Location(19.42857127110338, -99.16356656825693);EJEMPLO DEL PUNTO OBJETIVO DEL INMUEBLE AQUI DEBEN IR LAS COORDENADAS QUE DA EL API
            Location CurrentLocation = _location;// AQUI DEBE IR LAS COORDENADAS ACTUALES DEL GPS DEL MOVIL LAS CUALES LAS TENEMOS EN LA VARIABLE _location EN LA LINEA 109
                                                 //AL FINAL DEBERIA QUEDARTE DE LA SIGUIENTE FORMA PARA OBTENER LA UBICACION ACTUAL DEL MOVIL:Location CurrentLocation = _location;

            if(_location == null) {
                var message = LocationService.Message;
                await App.Current.MainPage.DisplayAlert("Mensaje", message, "Cerrar");
                return;
            }

            if(_selectionRadio == "A") {
                if(Math.Round(LocationService.CalcularDistancia(CurrentLocation, TargetDestination) * 1000, 2) > 400)//COMPROBAMOS QUE LA DISTANCIA NO SEA MAYOR A 100CM QUE EQUIVALE A 1 METRO, SI NECESITAS CAMBIAR LA DISTANCIA A COMPAR DEBES PONER EN CM LA DISTANCIA
                {//EL METODO CalcularDistancia() YA ME REGRESA UN VALOR CALCULADO EN KM X LO CUAL SE DEBE CONVERTIR A METROS
                 //Y ES POR ELLO QUE SE MULTIPLICA POR 1000 QUE SERIA 1KM Y LA CLASE MATH.ROUND ES PARA REDONDEAR LOS DECIMALES DE LOS METROS EN ESTE CASO A 2 DECIMALES
                    if(count == 0) {

                        count++;
                        var result = await App.Current.MainPage.DisplayAlert("Acción no permitida", "Parece que estas lejos de tu servicio, ¿Deseas registrarte en otro servicio?", "Si", "No");
                        if(result) {
                            var data = new Dictionary<string, object>
                            {
                                    {"NombreEmpleado", NombreCliente },
                                    {"IdEmpleado", IdEmpleado },
                                    {"Lat", Lat},
                                    {"Lng", Lng},
                                    {"IdInSele",IdInmueble},
                                    {"IdClSele",IdCliente}
                                };
                            await Shell.Current.GoToAsync("//SelectInmu", true, data);

                        } else {
                            count = 0;
                            IsBusy = false;
                            IsEnabled = true;
                            return;
                        }
                        IsBusy = false;
                        IsEnabled = true;
                        return;
                    }
                }
            } else if(_selectionRadio == "N") { }

            //if(await SendFiles()) { } else {
            //    await App.Current.MainPage.DisplayAlert("Error", "No fué posible guardar los archivos", "Cerrar");
            //    IsEnabled = true;
            //    IsBusy = false;
            //    return;
            //}

            IsEnabled = false;
            await GetPeriodo();
            RegistroModel registroModel = new RegistroModel {
                Adjuntos = PathFile == null ? "" : PathFile,
                Anio = (int)DateTime.Today.Year,
                Confirma = "BIOMETA",
                Cubierto = 0,
                Fecha = DateTime.Now,
                Idempleado = IdEmpleado,
                Idinmueble = IdInmueble,
                Idperiodo = _idPeriodo,
                Latitud = _location.Latitude.ToString(),
                Longitud = _location.Longitude.ToString(),
                Movimiento = _selectionRadio,
                RespuestaTexto = _respuestaTxt == null ? "" : _respuestaTxt,
                Tipo = _tipo,
                Foto = PathPhoto == null ? "" : PathPhoto,
            };

            int resp = await _httpHelper.PostBodyAsync<RegistroModel, int>(Constants.API_REGISTRO_BIOMETA, registroModel);

            if(resp == 0) {
                IsBusy = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al registrar", "Ok");
                return;
            }

            IsBusy = false;
            IsEnabled = false;
            await App.Current.MainPage.DisplayAlert("Mensaje", "Registrado correctamente", "Ok");
            NexTPage();

        } catch(Exception ex) {
            IsBusy = false;
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
        }
    }

    private async void NexTPage() {
        var data = new Dictionary<string, object>{
            {"NombreEmpleado", NombreCliente }
        };
        await Shell.Current.GoToAsync("//FormReg", true, data);
    }

    async Task GetPeriodo() {
        IsBusy = true;
        string url = $"{Constants.API_PERIODO_NOMINA}?Idempleado={UserSession.IdEmpleado}";
        var periodoEmpleado = await _httpHelper.GetAsync<ObservableCollection<PeriodoNominaModel.PeriodoClient>>(url);
        _idPeriodo = periodoEmpleado[0].id_periodo;
        _tipo = periodoEmpleado[0].descripcion;
        IsBusy = false;
    }

    [RelayCommand]
    private async Task LoadFile() {
        try {
            PickOptions options = new PickOptions();
            options.FileTypes = FilePickerFileType.Pdf;

            var resultOptions = await FilePicker.Default.PickMultipleAsync(options);
            if(resultOptions != null) {
                foreach(var result in resultOptions) {
                    if(result.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("doc", StringComparison.OrdinalIgnoreCase)) {
                        using var stream = await result.OpenReadAsync();
                        var file = ImageSource.FromStream(() => stream);
                        FileBase64 = ConvertToBase64(result.FullPath);
                    }
                    PathFile = result.FullPath;
                    archivos.Add(PathFile);
                }
            }

            //return result;
        } catch(Exception ex) {
            // The user canceled or something went wrong
        }
    }

    private byte ConvertToBase64(string path) {
        byte[] ImageData = File.ReadAllBytes(path);
        byte single;
        using(FileStream fs = new FileStream(path, FileMode.Open)) {
            single = (byte)fs.ReadByte();
        }
        return single;
        //byte v = ImageData;
        ////string base64String = Convert.ToBase64String(ImageData);
        //return ImageData;
    }

    [RelayCommand]
    private async Task Photo() {
        try {
            if(this.mediaPicker.IsCaptureSupported) {
                FileResult photo = await MediaPicker.CapturePhotoAsync();
                if(photo != null) {
                    string LocalFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                    using(Stream source = await photo.OpenReadAsync()) {
                        using FileStream localFile = File.OpenWrite(LocalFilePath);
                        await source.CopyToAsync(localFile);

                    }
                    var f = LocalFilePath;
                    PathPhoto = LocalFilePath;
                    Foto = ConvertToBase64(f);
                }
            }
        } catch(Exception ex) {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
        }
    }

    public async Task<bool> SendFiles() {
        if(PathPhoto != null)
            archivos.Add(PathPhoto);
        string urlarch = string.Empty;
        foreach(var item in archivos) {
            urlarch += item;
        }

        UrlFiles = await UploadFiles(archivos, "Doctos");
        string[] splits = UrlFiles.Split("|");// AQUI DEBEMOS INCLUIR EL SIGNO "|" SIN ESPAICIOS
        PathFile = string.Empty;
        foreach(string split in splits) {
            if(split.Contains(".pdf")) {
                // Si la extensión es PDF, asigna a pathFile y rompe el bucle
                PathFile += $"{split}|";
            } else if(split.Contains(".jpg") || split.Contains(".jpeg") || split.Contains(".png")) {
                // Si es una imagen (JPG, JPEG o PNG), asigna a pathPhoto y continúa el bucle
                PathPhoto = split;
            }
        }
        var EndPath = PathFile.TrimEnd('|');
        PathFile = EndPath;
        //await App.Current.MainPage.DisplayAlert("Url que se reciben del Servidor", $"{UrlFiles}", "Ok");
        return true;
    }

    public async Task<string> UploadFiles(List<string> files, string folderName) {

        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(folderName), "folderName");

        foreach(var file in files) {
            var fileContent = new StreamContent(File.OpenRead(file));
            formData.Add(fileContent, "files", Path.GetFileName(file));
        }

        string filePathList = await _httpHelper.PostMultipartAsync<string>(Constants.API_ENVIO_ARCHIVOS, formData);
        if(string.IsNullOrWhiteSpace(filePathList)) {
            await App.Current.MainPage.DisplayAlert("Error", $"Ocurrió un error al subir los archivos", "Cerrar");
            return null;
        }
        return filePathList;
    }
}