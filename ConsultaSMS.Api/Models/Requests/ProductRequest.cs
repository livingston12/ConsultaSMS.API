using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebConsultaSMS.Models.Requests
{
    public class AccountProductsRequest : Request
    {
        public string pCodCliente { get; set; }
        public string pTiposProductos { get; set; }
    }

    [DisplayName("AccountProductRequest")]
    public class ProductRequest : Request
    {
        [MaxLength(length: 4, ErrorMessage = "The maximun character of field {0} is {1}")]
        public string pTiposProductos { get; set; }
        internal string pCodCliente { get; set; }
    }
}
