using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebConsultaSMS.Models.Requests
{
    public class ProductDetailRequest
    {
        [NotMapped]
        internal string pTiposProductos { get; set; }

        [NotMapped]
        internal string pNumTarjeta
        {
            get => pNumCuenta;
        }

        [NotMapped]
        internal string pNumPrestamo
        {
            get => pNumCuenta;
        }

        [NotMapped]
        internal string pNumCertificado
        {
            get => pNumCuenta;
        }

        [NotMapped]
        public bool applePay
        {
            get => false;
        }

        [NotMapped]
        internal string pNumCuenta { get; set; }

        [
            Required,
            MaxLength(length: 250, ErrorMessage = "The maximun character of field {0} is {1}")
        ]
        public string pTelefono { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string pTexto { get; set; }
    }
}
