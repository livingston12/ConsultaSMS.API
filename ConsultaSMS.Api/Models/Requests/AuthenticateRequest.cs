using System.ComponentModel.DataAnnotations;

namespace WebConsultaSMS.Models
{
    public class AuthenticateRequest
    {
        [Required(ErrorMessage = "The {0} is required"), MaxLength(16)]
        public string pCedula { get; set; }

        [Required(ErrorMessage = "The {0} is required"), MaxLength(250)]
        public string pTelefono { get; set; }
    }

    public class AuthenticateInternalRequest
    {
        [Required(ErrorMessage = "The {0} is required"), MaxLength(250)]
        public string pPassword { get; set; }
    }
}
