using Asis_Batia.Helpers;

namespace Asis_Batia.Model.Jornaleros;

public class CoberturaModel {

    #region Coberturas por F, FJ, N, IEG, IRT, V
    public int IdPeriodo { get; set; }
    public int Anio { get; set; }
    public string Tipo { get; set; }
    public int IdEmpleado { get; set; }
    public DateTime Fecha { get; set; }
    public string Movimiento { get; set; }
    public int IdTurno { get; set; }
    public string Turno { get; set; }
    #endregion

    #region Coberturas por L 
    public int IdVacante { get; set; }
    #endregion

    public override string ToString() {
        if(IdVacante == 0) {
            string mov = Constants.GetRegisterType(Movimiento);
            return $"{mov} \t\t\t\t\t {Turno}";
        } else {
            return $"Vacantes \t\t\t\t\t {Turno}";
        }

    }
}