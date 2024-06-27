using Asis_Batia.Model;

namespace Asis_Batia.Helpers;

public class UserSession {

    static readonly string EMPLEADO_KEY = "Empleado key";
    static readonly string ID_EMPLEADO_KEY = "Id empleado key";
    static readonly string CLIENTE_KEY = "Cliente key";
    static readonly string ID_CLIENTE_KEY = "Id cliente key";
    static readonly string INMUEBLE_KEY = "Inmueble key";
    static readonly string ID_INMUEBLE_KEY = "Id inmueble key";
    static readonly string ESTADO_KEY = "Estado key";
    static readonly string ID_ESTADO_KEY = "Id estado key";
    static readonly string SHOW_TERMS_CONDITIONS_KEY = "Show terms and conditions";

    public static string Empleado {
        get => Preferences.Default.ContainsKey(EMPLEADO_KEY)
            ? (string)Preferences.Default.Get(EMPLEADO_KEY, string.Empty) : string.Empty;
        set => Preferences.Default.Set(EMPLEADO_KEY, value);
    }

    public static int IdEmpleado {
        get => Preferences.Default.ContainsKey(ID_EMPLEADO_KEY)
            ? (int)Preferences.Default.Get(ID_EMPLEADO_KEY, 0) : 0;
        set => Preferences.Default.Set(ID_EMPLEADO_KEY, value);
    }

    public static string Cliente {
        get => Preferences.Default.ContainsKey(CLIENTE_KEY)
            ? (string)Preferences.Default.Get(CLIENTE_KEY, string.Empty) : string.Empty;
        set => Preferences.Default.Set(CLIENTE_KEY, value);
    }

    public static int IdCliente {
        get => Preferences.Default.ContainsKey(ID_CLIENTE_KEY)
            ? (int)Preferences.Default.Get(ID_CLIENTE_KEY, 0) : 0;
        set => Preferences.Default.Set(ID_CLIENTE_KEY, value);
    }

    public static string Inmueble {
        get => Preferences.Default.ContainsKey(INMUEBLE_KEY)
            ? (string)Preferences.Default.Get(INMUEBLE_KEY, string.Empty) : string.Empty;
        set => Preferences.Default.Set(INMUEBLE_KEY, value);
    }

    public static int IdInmueble {
        get => Preferences.Default.ContainsKey(ID_INMUEBLE_KEY)
            ? (int)Preferences.Default.Get(ID_INMUEBLE_KEY, 0) : 0;
        set => Preferences.Default.Set(ID_INMUEBLE_KEY, value);
    }

    public static string Estado {
        get => Preferences.Default.ContainsKey(ESTADO_KEY)
            ? (string)Preferences.Default.Get(ESTADO_KEY, string.Empty) : string.Empty;
        set => Preferences.Default.Set(ESTADO_KEY, value);
    }

    public static int IdEstado {
        get => Preferences.Default.ContainsKey(ID_ESTADO_KEY)
            ? (int)Preferences.Default.Get(ID_ESTADO_KEY, 0) : 0;
        set => Preferences.Default.Set(ID_ESTADO_KEY, value);
    }

    public static bool ShowTermsConditions {
        get => Preferences.Default.ContainsKey(SHOW_TERMS_CONDITIONS_KEY)
            ? (bool)Preferences.Default.Get(SHOW_TERMS_CONDITIONS_KEY, true) : true;
        set => Preferences.Default.Set(SHOW_TERMS_CONDITIONS_KEY, value);
    }

    public static void SetData(InfoEmpleadoModel data) {
        Empleado = data.empleado;
        IdEmpleado = data.idEmpleado;
        Cliente = data.cliente;
        IdCliente = data.idCliente;
        Inmueble = data.puntoAtencion;
        IdInmueble = data.idInmueble;
        Estado = data.estado;
        IdEstado = data.idEstado;
    }

    public static void ClearSession() {
        Preferences.Default.Clear();
        ShowTermsConditions = false;
    }
}