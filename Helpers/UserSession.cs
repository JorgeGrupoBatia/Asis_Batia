﻿using Asis_Batia.Model;

namespace Asis_Batia.Helpers;

public class UserSession {

    static readonly string EMPLEADO_KEY = "Empleado key";
    static readonly string ID_EMPLEADO_KEY = "Id empleado key";
    static readonly string CLIENTE_KEY = "Cliente key";
    static readonly string ID_CLIENTE_KEY = "Id cliente key";
    static readonly string INMUEBLE_KEY = "Inmueble key";
    static readonly string ID_INMUEBLE_KEY = "Id inmueble key";
    static readonly string LATITUDE_INMUEBLE_KEY = "Latitude inmueble";
    static readonly string LONGITUDE_INMUEBLE_KEY = "Longitud inmueble";
    static readonly string ESTADO_KEY = "Estado key";
    static readonly string ID_ESTADO_KEY = "Id estado key";
    static readonly string SHOW_TERMS_CONDITIONS_KEY = "Show terms and conditions";
    static readonly string TURNO_KEY = "Turno key";
    static readonly string ID_TURNO_KEY = "Id turno key";
    static readonly string IS_BIOMETRICS_ACTIVATED_KEY = "Biometric activated key";

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

    public static string LongitudInmueble {
        get => Preferences.Default.ContainsKey(LATITUDE_INMUEBLE_KEY)
            ? (string)Preferences.Default.Get(LATITUDE_INMUEBLE_KEY, string.Empty) : string.Empty;
        set => Preferences.Default.Set(LATITUDE_INMUEBLE_KEY, value);
    }

    public static string LatitudeInmueble {
        get => Preferences.Default.ContainsKey(LONGITUDE_INMUEBLE_KEY)
            ? (string)Preferences.Default.Get(LONGITUDE_INMUEBLE_KEY, string.Empty) : string.Empty;
        set => Preferences.Default.Set(LONGITUDE_INMUEBLE_KEY, value);
    }

    public static bool ShowTermsConditions {
        get => Preferences.Default.ContainsKey(SHOW_TERMS_CONDITIONS_KEY)
            ? (bool)Preferences.Default.Get(SHOW_TERMS_CONDITIONS_KEY, true) : true;
        set => Preferences.Default.Set(SHOW_TERMS_CONDITIONS_KEY, value);
    }

    public static string Turno {
        get => Preferences.Default.ContainsKey(TURNO_KEY)
            ? (string)Preferences.Default.Get(TURNO_KEY, string.Empty) : string.Empty;
        set => Preferences.Default.Set(TURNO_KEY, value);
    }

    public static int IdTurno {
        get => Preferences.Default.ContainsKey(ID_TURNO_KEY)
            ? (int)Preferences.Default.Get(ID_TURNO_KEY, 0) : 0;
        set => Preferences.Default.Set(ID_TURNO_KEY, value);
    }

    public static bool IsBiometricsActivated {
        get => Preferences.Default.ContainsKey(IS_BIOMETRICS_ACTIVATED_KEY)
            ? (bool)Preferences.Default.Get(IS_BIOMETRICS_ACTIVATED_KEY, false) : false;
        set => Preferences.Default.Set(IS_BIOMETRICS_ACTIVATED_KEY, value);
    }

    public static bool EsEmpleadoElektra {
        get => IdCliente == 130;
    }

    public static bool EsEmpleadoAeropuerto {
        get => IdCliente == 2387;
    }

    public static bool EsEmpleadoTerminal1 {
        get => IdCliente == 14029 || IdCliente == 14189 || IdCliente == 14191 || IdCliente == 14192 
            || IdCliente == 14193 || IdCliente == 14194 || IdCliente == 14195 || IdCliente == 14196;
    }

    public static bool EsTurnoNocturno {
        get => IdTurno == 3;
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
        LatitudeInmueble = data.LatitudInmueble;
        LongitudInmueble = data.LongitudInmueble;
        Turno = data.Turno;
        IdTurno = data.IdTurno;
    }

    public static void ClearSession() {
        bool isActivatedBiometric = IsBiometricsActivated;
        Preferences.Default.Clear();
        IsBiometricsActivated = isActivatedBiometric;
        ShowTermsConditions = false;
    }
}