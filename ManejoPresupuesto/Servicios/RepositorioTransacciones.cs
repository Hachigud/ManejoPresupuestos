﻿using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int idTransaccion, int usuarioId);
        Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
    }
    public class RepositorioTransacciones: IRepositorioTransacciones
    {


        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }



        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar", new {transaccion.UsuarioId, transaccion.FechaTransaccion, transaccion.Monto,transaccion.CategoriaId,
                                                            transaccion.CuentaId, transaccion.Nota}, commandType: System.Data.CommandType.StoredProcedure);


            transaccion.IdTransaccion= id;

        }


        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId (ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"SELECT t.IdTransaccion, t.Monto, t.FechaTransaccion, c.Nombre as Categoria, cu.Nombre as Cuenta, c.TipoOperacionId
                                    FROM Transacciones t 
                                    INNER JOIN Categorias c 
                                    ON c.IdCategoria = t.CategoriaId
                                    INNER JOIN Cuentas cu
                                    ON cu.IdCuentas = t.CuentaId
                                    WHERE t.CuentaId = @CuentaId AND t.UsuarioId = @UsuarioId AND FechaTransaccion BETWEEN @FechaInicio and @FechaFin", modelo);


        }



        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"SELECT t.IdTransaccion, t.Monto, t.FechaTransaccion, c.Nombre as Categoria, cu.Nombre as Cuenta, c.TipoOperacionId, Nota
                                    FROM Transacciones t 
                                    INNER JOIN Categorias c 
                                    ON c.IdCategoria = t.CategoriaId
                                    INNER JOIN Cuentas cu
                                    ON cu.IdCuentas = t.CuentaId
                                    WHERE t.UsuarioId = @UsuarioId AND FechaTransaccion BETWEEN @FechaInicio and @FechaFin
                                    ORDER BY t.FechaTransaccion DESC", modelo);


        }

            public async Task Actualizar (Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("Transacciones_Actualizar", new
            {
                transaccion.IdTransaccion,
                transaccion.FechaTransaccion,
                transaccion.Monto,   
                transaccion.CuentaId,     
                transaccion.CategoriaId,
                transaccion.Nota,
                montoAnterior,
                cuentaAnteriorId
            }, commandType: System.Data.CommandType.StoredProcedure);


        }


        public async Task<Transaccion> ObtenerPorId (int idTransaccion, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"SELECT Transacciones.*, cat.TipoOperacionId
                                    FROM Transacciones
                                    INNER JOIN Categorias cat
                                    ON cat.IdCategoria = Transacciones.CategoriaId
                                    WHERE Transacciones.IdTransaccion = @IdTransaccion AND Transacciones.UsuarioId = @UsuarioId", new {idTransaccion,usuarioId});

        }


        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana (ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"SELECT datediff(d, @fechaInicio, FechaTransaccion) / 7 + 1 as Semana, SUM(Monto) as Monto, cat.TipoOperacionId
                                    FROM Transacciones
                                    INNER JOIN  Categorias cat
                                    On cat.IdCategoria = Transacciones.CategoriaId
                                    WHERE Transacciones.UsuarioId = @usuarioId AND  FechaTransaccion BETWEEN @fechaInicio AND @fechaFin
                                    GROUP BY datediff(d, @fechaInicio, FechaTransaccion) / 7 + 1 ,  cat.TipoOperacionId", modelo);

        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ResultadoObtenerPorMes>(@"SELECT  MONTH(FechaTransaccion) as Mes,
                                    SUM(Monto) as Monto, cat.TipoOperacionId
                                    FROM Transacciones
                                    INNER JOIN Categorias cat
                                    ON cat.IdCategoria = Transacciones.CategoriaId
                                    WHERE Transacciones.UsuarioId = @usuarioId AND YEAR(FechaTransaccion) = @Año
                                    GROUP BY MONTH(FechaTransaccion), cat.TipoOperacionId",new {usuarioId,año});

        }




        public async Task Borrar (int IdTransaccion)
        {

            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("Transacciones_Borrar",
                new { IdTransaccion }, commandType: System.Data.CommandType.StoredProcedure);
        }

    }
}
