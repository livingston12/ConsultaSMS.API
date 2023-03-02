using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebConsultaSMS.Models.Entities
{
    public class GeneralParameterEntity : Entity
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(75)]
        public string Name { get; set; }
        [Required, MaxLength(25)]
        public string NameAbrev { get; set; }
        [Required, MaxLength(150)]
        public string Description { get; set; }
        [Required]
        public string Value { get; set; }
        [DefaultValue("False")]
        public bool Active { get; set; }
    }
}
