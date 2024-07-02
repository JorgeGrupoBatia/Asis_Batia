using Asis_Batia.Helpers;
using System.Text.Json.Serialization;

namespace Asis_Batia.Model;

public class MovimientoModel {
    public DateTime Fecha { get; set; }
    public string Movimiento { get; set; }

    [JsonIgnore]
    public string Descripcion {
        get => GetTipoRegistro(Movimiento);
    }

    public static string GetTipoRegistro(string nomenclatura) {
        switch(nomenclatura) {
            case "A":
                return Constants.INICIO_LABORES;
            case "A2":
                return Constants.SALIDA_COMER;
            case "A3":
                return Constants.ENTRADA_COMER;
            case "A4":
                return Constants.FIN_LABORES;
            case "N":
                return Constants.DESCANSO;
            default:
                return "";
        }
    }
}