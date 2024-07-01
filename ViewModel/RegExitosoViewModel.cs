using Asis_Batia.Helpers;
using MauiPopup;
using System.Windows.Input;

namespace Asis_Batia.ViewModel;

public class RegExitosoViewModel : BaseViewModel, IQueryAttributable {
    private string _nombreCliente;

    public string NombreCliente {
        get { return _nombreCliente; }
        set { _nombreCliente = value; OnPropertyChanged(); }
    }

    private DateTime _fecha;

    public DateTime Fecha {
        get { return _fecha; }
        set { _fecha = value; OnPropertyChanged(); }
    }

    public ICommand AcceptCommand { get; set; }

    public RegExitosoViewModel() {
        NombreCliente = UserSession.Empleado;
        AcceptCommand = new Command(Accept);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        Fecha = DateTime.Now;
    }

    private void Accept() {
        PopupAction.ClosePopup();
    }
}