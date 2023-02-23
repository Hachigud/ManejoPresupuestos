using ManejoPresupuesto.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta   /*: IValidatableObject*/
    {
        public int IdTipoCuenta { get; set; }

        [Required(ErrorMessage = "Debe completar el Campo {0} ;).")]
        [PrimeraLetraMayuscula]
        [Remote(action:"VerificarExisteTipoCuenta", controller: "TiposCuentas")]
        public string Nombre { get; set; }


        public int UsuarioId { get; set; }


        public int Orden { get; set; }



        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if(Nombre != null && Nombre.Length > 0)
        //    {
        //        var primeraLetra = Nombre[0].ToString();                 // Valdicon a nivel modelo

        //        if (primeraLetra != primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra debe ser mayuscula",
        //                new[] { nameof(Nombre) });
        //        }

        //    }
        //}
    }
}
