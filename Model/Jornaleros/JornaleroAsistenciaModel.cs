namespace Asis_Batia.Model.Jornaleros;

public class JornaleroAsistenciaModel {
    public int IdPeriodo { get; set; }
    public string Tipo { get; set; }
    public int Anio { get; set; }
    public int IdCliente { get; set; }
    public int IdInmueble { get; set; }
    public int IdJornalero { get; set; }
    public int IdTurno { get; set; }
    //public DateTime Fecha { get; set; }
    //public int Importe { get; set; }
    public int TipoAsistencia { get; set; }
    public string TipoMovimiento { get; set; }
    public int IdVacante { get; set; }
    public int IdEvento { get; set; }
    public int IdEmpleado { get; set; }
}
