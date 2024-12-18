using Asis_Batia.Model;

namespace Asis_Batia.Helpers;

public static class Constants {

    public static string APP_NAME = AppInfo.Name;
    public static string APP_VERSION_STRING = AppInfo.VersionString;

    #region API URI´s
    public const string API_BASE_URL = "https://www.singa.com.mx:5500/api/";
    public const string API_EMPLEADO_APP = "EmpleadoApp";
    public const string API_PRECARGAR_EMPLEADOS = "EmpleadosPrecargados";
    public const string API_PRECARGAR_MOVIMIENTOS = "MovimientosBiometaAeropuerto";
#if DEBUG
    public const string API_MOVIMIENTOS_BIOMETA = "MovimientosBiometaPrueba";
    public const string API_REGISTRO_BIOMETA = "RegistroBiometaPrueba";
    public const string API_REGISTRO_MASIVO_BIOMETA = "RegistroBiometaMasivoPrueba";
#elif RELEASE
    public const string API_MOVIMIENTOS_BIOMETA = "MovimientosBiometa";
    public const string API_REGISTRO_BIOMETA = "RegistroBiometaN";
    public const string API_REGISTRO_MASIVO_BIOMETA = "RegistroBiometaMasivo";
#endif
    public const string API_CLIENTE = "cliente";
    public const string API_INMUEBLES = "Inmueble";
    public const string API_ENVIO_ARCHIVOS = "FilesAsis/CargaMul";
    #endregion

    #region SQLite
    public const string DATABASE_FILE_NAME = "BiometaDB.db3";
    public const SQLite.SQLiteOpenFlags FLAGS = SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache;
    public static string DATABASE_PATH = Path.Combine(FileSystem.AppDataDirectory, DATABASE_FILE_NAME);
    #endregion

    #region Literales
    public const string JORNALEROS_PAGE_TITLE = "Asistencia Jornaleros";

    public const string SI = "Sí";
    public const string NO = "No";
    public const string RECHAZAR = "Rechazar";
    public const string CANCELAR = "Cancelar";
    public const string ACEPTAR = "Aceptar";
    public const string BIOMETA = "BIOMETA";
    public const string EMPLEADO = "EMPLEADO";
    public const string EMPRESA_CLIENTE = "EMPRESA/CLIENTE";
    public const string ESTADO = "ESTADO";
    public const string TURNO = "TURNO";
    public const string PUNTO_ATENCION = "PUNTO DE ATENCION O AREA DE TRABAJO";
    public const string SIGUIENTE = "SIGUIENTE";
    public const string REGISTRAR_ENTRADA_SALIDA = "Registrar entrada / salida";
    public const string ARCHIVO = "Archivo";
    public const string FOTO = "Foto";
    public const string ULTIMO_REGISTRO = "Último registro";
    public const string NO_COINCIDE_BIOMETRIA = "Los datos biométricos no coinciden";
    public const string INGRESE_DATOS_BIOMETRICOS = "Captura de datos biométricos";
    public const string COLOQUE_ROSTRO_HUELLA = "Coloque su rostro frente a la cámara o ingrese sus huella dactilar para verificar identidad";
    public const string SIN_CONEXION = "Sin conexión a Internet";
    public const string ERROR = "Error";
    public const string PRECARGANDO_DATOS = "Precargando datos ...";
    public const string ENVIANDO_REGISTROS = "Enviando registros ...";
    public const string DATOS_PRECARGADOS_OK = "¡ Datos precargados correctamente !";
    public const string DATOS_PRECARGADOS_ERROR = "¡ Error al precargar los datos !";
    public const string REGISTROS_SINCRONIZADOS_OK = "¡ Registros sincronizados correctamente !";
    public const string REGISTROS_SINCRONIZADOS_ERROR_1 = "Error al enviar los registros. Vuelva a intentarlo.";
    public const string REGISTROS_SINCRONIZADOS_ERROR_2 = "Error al enviar los registros. Precargue los datos nuevamente.";
    public const string SOY_EMPLEADO = "Soy Empleado";
    public const string SOY_JORNALERO = "Soy Jornalero";
    public const string ID_EMPLEADO = "ID de Empleado";
    public const string ID_JORNALERO = "ID de Jornalero";

    public const string A = "A";
    public const string A2 = "A2";
    public const string A3 = "A3";
    public const string A4 = "A4";
    public const string N = "N";
    public const string D = "D";
    public const string F = "F";
    public const string FJ = "FJ";
    public const string IEG = "IEG";
    public const string IRT = "IRT";
    public const string V = "V";

    public const string INICIO_LABORES = "Inicio de labores";
    public const string SALIDA_COMER = "Salida a comer";
    public const string ENTRADA_COMER = "Entrada de comer";
    public const string FIN_LABORES = "Fin de labores";
    public const string DESCANSO = "Descanso";
    public const string DOBLETE = "Doblete";
    public const string FALTA = "Falta";
    public const string FALTA_JUSTIFICADA = "Falta justificada";
    public const string INCAPACIDAD_ENFERMEDAD_GENERAL = "Incapacidad por enfermedad general";
    public const string INCAPACIDAD_RIESGO_TRABAJO = "Incapacidad por riesgo de trabajo";
    public const string VACACIONES = "Vacaciones";

    public const string SIN_REGISTRO = "-";

    public static string GetNextRegister(MovimientoModel ultimoRegistro) {

        bool esMismoDia = ultimoRegistro.Fecha.Day == DateTime.Now.Day;

        if(UserSession.EsTurnoNocturno) {
            switch(ultimoRegistro.Movimiento) {
                case A:
                    return A4;
                case A4:
                    return A;
                default: // A2, A3, N, D, F, FJ, IEG, IRT, V
                    return A;
            }
        } else {
            if(!UserSession.EsEmpleadoElektra) {
                switch(ultimoRegistro.Movimiento) {
                    case A:
                        return esMismoDia ? A4 : A;
                    case A4:
                        return esMismoDia ? SIN_REGISTRO : A;
                    default: // A2, A3, N, D, F, FJ, IEG, IRT, V
                        return esMismoDia ? SIN_REGISTRO : A;
                }
            } else {
                switch(ultimoRegistro.Movimiento) {
                    case A:
                        return esMismoDia ? A2 : A;
                    case A2:
                        return esMismoDia ? A3 : A;
                    case A3:
                        return esMismoDia ? A4 : A;
                    case A4:
                        return esMismoDia ? SIN_REGISTRO : A;
                    default: // N, D, F, FJ, IEG, IRT, V
                        return esMismoDia ? SIN_REGISTRO : A;
                }
            }
        }
    }

    public static string GetRegisterType(string nomenclatura) {
        switch(nomenclatura) {
            case A:
                return INICIO_LABORES;
            case A2:
                return SALIDA_COMER;
            case A3:
                return ENTRADA_COMER;
            case A4:
                return FIN_LABORES;
            case N:
                return DESCANSO;
            case D:
                return DOBLETE;
            case F:
                return FALTA;
            case FJ:
                return FALTA_JUSTIFICADA;
            case IEG:
                return INCAPACIDAD_ENFERMEDAD_GENERAL;
            case IRT:
                return INCAPACIDAD_RIESGO_TRABAJO;
            case V:
                return VACACIONES;
            default:
                return string.Empty;
        }
    }
    #endregion

    #region Keys
    public const string NOMENCLATURA_KEY = "Nomenclatura key";
    public const string LOCATION_KEY = "Location key";
    public const string INICIO_LABORES_KEY = "Inicio labores key";
    #endregion

    #region Aviso de privacidad
    public const string AVISO_PRIVACIDAD_PARTE_1 =
        "Gracias por utilizar nuestra aplicación móvil. La protección de su privacidad es de suma importancia para nosotros. Por lo tanto, hemos desarrollado esta Política de Privacidad para que comprenda cómo recopilamos, usamos, comunicamos, divulgamos y utilizamos su información personal."
        + "\n\nLe recomendamos encarecidamente que revise cuidadosamente esta Política de Privacidad antes de usar la aplicación o proporcionar cualquier tipo de información personal."
        + "\n\nRecopilación de información en BIOMETA."
        + "\n1.Datos personales:"
        + "\nLa aplicación recopila y utiliza datos personales como nombre, ID de empleado y periodo nominal. Estos datos se utilizan para identificarle como usuario y para lograr correctamente su registro de asistencia laboral."
        + "\n\n2.Datos de ubicación:"
        + "\nLa aplicación recopila y utiliza su ubicación geográfica con la dirección de área de trabajo para verificar que su registro se haga correctamente en el inmueble asignado."
        + "\n\n3.Archivos e imágenes:"
        + "\nLa aplicación puede acceder y manejar archivos como PDF e imágenes almacenadas en su dispositivo móvil para enviar un registro de asistencia completo y con más información."
        + "\n\nEn concreto, Grupo Batia podrá recabar y tratar las siguientes categorías de datos personales:"
        + "\n• Datos de identificación \n• Datos de contacto \n• Datos de navegación, dispositivos y geolocalización \n• Datos personales. "
        + "\n\nPara cumplir con las finalidades descritas en este Aviso, Grupo Batia no trata ni solicita datos personales sensibles."
        + "\n\nDatos personales de terceros"
        + "\nSi usted entrega a Grupo Batia datos personales de terceros para verificación de posibles conflictos de interés, deberá informarles sobre la existencia del tratamiento de sus datos personales y el contenido de este Aviso de Privacidad. Si proporciona dicha información, "
        + "también manifiesta con su entrega que cuenta previamente con el consentimiento de sus titulares para proporcionar su información a Grupo Batia y que los mismos son correctos y completos."
        + "\n\nPara utilizar nuestra aplicación y continuar con su registro, debe aceptar y autorizar que está de acuerdo y es consciente del manejo y uso de sus datos.";

    public const string AVISO_PRIVACIDAD_PARTE_2 = "\n\nAl hacer clic en Aceptar o continuar con su registro, usted otorga su consentimiento explícito para el manejo de sus datos personales."
        + "\n\nLe recomendamos que lea detenidamente nuestra política de privacidad que se encuentran al inicio de esta ventana.";
    public const string AVISO_PRIVACIDAD_PARTE_3 = "\n\nAgradecemos su confianza en nosotros y estamos comprometidos en proteger toda información utilizada en BIOMETA.";

    public const string AVISO_PRIVACIDAD_MOSTRAR_POUP = $"{AVISO_PRIVACIDAD_PARTE_1}{AVISO_PRIVACIDAD_PARTE_2}{AVISO_PRIVACIDAD_PARTE_3}";
    public const string AVISO_PRIVACIDAD_MOSTRAR_PAGE = $"{AVISO_PRIVACIDAD_PARTE_1}{AVISO_PRIVACIDAD_PARTE_3}";
    #endregion
}