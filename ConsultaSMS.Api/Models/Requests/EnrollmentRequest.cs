using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebConsultaSMS.Models
{
    public class EnrollmentRequest
    {
        [Required(ErrorMessage = "The {0} is required"), MaxLength(4)]
        public string pPin { get; set; }
    }
}
