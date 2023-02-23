using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int idTipoCuenta);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int idTipoCuenta, int UsuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenados);
    }

    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {

        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task  Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("TiposCuentasInsertar",
                new {UsuarioId = tipoCuenta.UsuarioId, nombre = tipoCuenta.Nombre},
                commandType: System.Data.CommandType.StoredProcedure);

            tipoCuenta.IdTipoCuenta = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1
                                            FROM TiposCuentas
                                            WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId;", new {nombre, usuarioId});

            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<TipoCuenta>(@"SELECT idTipoCuenta, Nombre,Orden
                                            FROM TiposCuentas
                                            WHERE UsuarioId = @UsuarioId 
                                            ORDER BY Orden",new {usuarioId});
        }


        public async Task Actualizar (TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                            SET Nombre = @Nombre
                                            WHERE idTipoCuenta = @idTipoCuenta", tipoCuenta);

        }
        public async Task<TipoCuenta> ObtenerPorId(int idTipoCuenta,int UsuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT idTipoCuenta, Nombre, Orden
                                            FROM TiposCuentas
                                            WHERE idTipoCuenta = @idTipoCuenta AND UsuarioId = @UsuarioId;", new { idTipoCuenta, UsuarioId });
        }


        public async Task Borrar(int idTipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"DELETE TiposCuentas
                                            WHERE idTipoCuenta = @idTipoCuenta", new {idTipoCuenta});

        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            var query = "UPDATE TiposCuentas SET Orden = @Orden WHERE idTipoCuenta = @idTipoCuenta;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }


    }
}
