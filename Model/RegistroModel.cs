using Newtonsoft.Json;
using SQLite;

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

    [JsonProperty("Id")]
    public int IdRegistro { get; set; }
}
