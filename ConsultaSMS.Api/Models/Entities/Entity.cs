using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebConsultaSMS.Models.Entities
{
    public class Entity
    {
        [Required, DisplayName("Created Date"), DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? CreatedUserId { get; set; }
    }
}