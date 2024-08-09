using Asis_Batia.Helpers;
using System.Text.Json.Serialization;

namespace Asis_Batia.Model;

public class MovimientoModel {
    public DateTime Fecha { get; set; }
    public string Movimiento { get; set; }

    [JsonIgnore]
    public string Descripcion {
        get => Constants.GetRegisterType(Movimiento);
    }   
}