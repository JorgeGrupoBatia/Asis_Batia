using System.Text.Json.Serialization;

namespace Asis_Batia.Model;

public class MovimientoModel {
    public DateTime Fecha { get; set; }
    public string Movimiento { get; set; }

    [JsonIgnore]
    public string Descripcion {
        get {
            switch(Movimiento) {
                case "A":
                    return "Inicio de labores :";
                case "A2":
                    return "Salida a comer :";
                case "A3":
                    return "Entrada de comer :";
                case "A4":
                    return "Fin de labores :";
                default:
                    return "";
            }
        }
    }
}