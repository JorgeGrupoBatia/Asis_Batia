namespace Asis_Batia.Helpers;

public static class Constants {

    #region API URI´s
    public const string API_BASE_URL = "https://www.singa.com.mx:5500/api/";
    public const string API_EMPLEADO_APP = "EmpleadoApp";
    public const string API_INMUEBLES = "Inmueble";
    public const string API_ENVIO_ARCHIVOS= "FilesAsis/CargaMul";
    public const string API_REGISTRO_BIOMETA = "RegistroBiometaN";
    public const string API_MOVIMIENTOS_BIOMETA = "MovimientosBiometa";
    public const string API_CLIENTE = "cliente";
    #endregion

    #region Literales
    public const string INICIO_LABORES = "Inicio de labores";
    public const string SALIDA_COMER = "Salida a comer";
    public const string ENTRADA_COMER = "Entrada de comer";
    public const string FIN_LABORES = "Fin de labores";
    public const string DESCANSO = "Descanso";
    #endregion

    #region Keys
    public const string NOMENCLATURA_KEY = "Nomenclatura key";
    public const string LOCATION_KEY = "Location key";
    #endregion

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

    public const string AVISO_PRIVACIDAD_MOSTRAR_POUP=$"{AVISO_PRIVACIDAD_PARTE_1}{AVISO_PRIVACIDAD_PARTE_2}{AVISO_PRIVACIDAD_PARTE_3}";
    public const string AVISO_PRIVACIDAD_MOSTRAR_PAGE=$"{AVISO_PRIVACIDAD_PARTE_1}{AVISO_PRIVACIDAD_PARTE_3}";
} 