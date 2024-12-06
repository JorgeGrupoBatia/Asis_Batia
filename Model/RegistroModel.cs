using SQLite;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using Asis_Batia.Helpers;

namespace Asis_Batia.Model;

public class RegistroModel {
    [PrimaryKey, AutoIncrement]
    [JsonIgnore]
    public int Id { get; set; }
    public int Idempleado { get; set; }
    public string Confirma { get; set; }
    public string Movimiento { get; set; }
    public int Cubierto { get; set; }
    public string Adjuntos { get; set; }
    public string RespuestaTexto { get; set; }
    public string Latitud { get; set; }
    public string Longitud { get; set; }
    public string Foto { get; set; }
    public DateTime Fecha { get; set; }
    public int IdPeriodo { get; set; }
    public int Anio { get; set; }
    public string Tipo { get; set; }
    [JsonIgnore]
    public string Cobertura {
        get {
            string mov = Constants.GetRegisterType(Movimiento);
            return $"{mov} \t\t\t\t\t {Fecha.ToString("dd-MM-yyyy")}";
        }
    }
}
