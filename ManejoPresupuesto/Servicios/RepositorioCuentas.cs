using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioCuentas: IRepositorioCuentas
    {

        public readonly string connectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuentas (Nombre, TipoCuentaId, Descripcion, Balance) VALUES (@Nombre, @TipoCuentaId, @Descripcion, @Balance); 
                                        SELECT SCOPE_IDENTITY();", cuenta);


            cuenta.IdCuentas = id;
        }


        public async Task<IEnumerable<Cuenta>> Buscar (int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Cuenta>(@"
                                    SELECT Cuentas.IdCuentas, Cuentas.Nombre, Balance,tc.Nombre as TipoCuenta
                                    FROM Cuentas 
                                    INNER JOIN TiposCuentas tc
                                    ON tc.idTipoCuenta = Cuentas.TipoCuentaId
                                    WHERE tc.UsuarioId = @UsuarioId
                                    ORDER BY tc.Orden", new { usuarioId});
        }
        

        public async Task<Cuenta> ObtenerPorId (int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT Cuentas.IdCuentas, Cuentas.Nombre, Balance, Descripcion, Cuentas.TipoCuentaId
                                    FROM Cuentas 
                                    INNER JOIN TiposCuentas tc
                                    ON tc.idTipoCuenta = Cuentas.TipoCuentaId
                                    WHERE tc.UsuarioId = @UsuarioId AND Cuentas.IdCuentas = @Id;", new {id, usuarioId});


        }


        public async Task Actualizar (CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Cuentas
                            SET Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion, TipoCuentaId = @TipoCuentaId
                            WHERE IdCuentas = @IdCuentas;", cuenta);
        }


        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE Cuentas WHERE IdCuentas = @Id", new { id });
        }

    }
}
