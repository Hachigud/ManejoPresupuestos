﻿namespace ManejoPresupuesto.Models
{
    public class TransaccionActualizacionViewModel: TransaccionCreacionViewModel
    {

        public int CuentaAnteriorId { get; set; }

        public decimal MontoAnteriorId { get; set; }

        public string UrlRetorno { get; set; }

    }
}
