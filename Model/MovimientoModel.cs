using Asis_Batia.Helpers;
using SQLite;
using System.Text.Json.Serialization;

namespace Asis_Batia.Model;

public class MovimientoModel {

    [PrimaryKey]
    [JsonIgnore]
    public int IdEmpleado { get; set; }
    public DateTime Fecha { get; set; }
    public string Movimiento { get; set; }

    [JsonIgnore]
    public string Descripcion {
        get => Constants.GetRegisterType(Movimiento);
    }   
}