using Asis_Batia.Helpers;
using Asis_Batia.Model;
using SQLite;

namespace Asis_Batia.Data;

public class DbContext {

    SQLiteAsyncConnection _dbConn;

    public DbContext() { }

    async Task Init() {
        if(_dbConn is not null) {
            return;
        }

        _dbConn = new SQLiteAsyncConnection(Constants.DATABASE_PATH, Constants.FLAGS);
        await _dbConn.CreateTableAsync<RegistroModel>();
        await _dbConn.CreateTableAsync<InfoEmpleadoModel>();
        await _dbConn.CreateTableAsync<MovimientoModel>();
    }

    #region InfoEmpleadoModel
    public async Task<int> DeleteAllEmpleadosAsync() {
        await Init();
        return await _dbConn.DeleteAllAsync<InfoEmpleadoModel>();
    }

    public async Task<int> SaveAllEmpleadosAsync(List<InfoEmpleadoModel> listEmpleados) {
        await Init();
        return await _dbConn.InsertAllAsync(listEmpleados);
    }

    public async Task<InfoEmpleadoModel> GetEmpleadoAsync(string idEmpleadoString) {
        await Init();

        bool isValid = int.TryParse(idEmpleadoString, out int idEmpleado);
        if(!isValid) {
            return null;
        }

        return await _dbConn.Table<InfoEmpleadoModel>().Where(e => e.idEmpleado == idEmpleado).FirstOrDefaultAsync();
    }
    #endregion

    #region MovimientoModel
    public async Task<int> DeleteAllMovimientosAsync() {
        await Init();
        return await _dbConn.DeleteAllAsync<MovimientoModel>();
    }

    public async Task<int> SaveAllMovimientosAsync(List<MovimientoModel> listMovimientos) {
        await Init();
        return await _dbConn.InsertAllAsync(listMovimientos);
    }

    public async Task<MovimientoModel> GetMovimientoAsync(int idEmpleado) {
        await Init();
        return await _dbConn.Table<MovimientoModel>().Where(m => m.IdEmpleado == idEmpleado).FirstOrDefaultAsync();
    }

    public async Task<int> SaveMovimientoAsync(MovimientoModel movimiento) {
        await Init();

        MovimientoModel existsMov = await GetMovimientoAsync(movimiento.IdEmpleado);

        if(existsMov is null) {
            return await _dbConn.InsertAsync(movimiento);
        } else {
            return await _dbConn.UpdateAsync(movimiento);
        }
    }
    #endregion

    #region RegistroModel    
    public async Task<int> SaveRegisterAsync(RegistroModel registro) {
        await Init();
        return await _dbConn.InsertAsync(registro);
    }

    public async Task<int> DeleteAllRegisterAsync() {
        await Init();
        return await _dbConn.DeleteAllAsync<RegistroModel>();
    }

    public async Task<List<RegistroModel>> GetAllRegistersAsync() {
        await Init();
        return await _dbConn.Table<RegistroModel>().ToListAsync();
    }
    #endregion
}