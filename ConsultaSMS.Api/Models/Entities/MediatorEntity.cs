using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebConsultaSMS.Models.Entities
{
    public class MediatorEntity : Entity
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MaxLength(150)]
        public string Description { get; set; }
        [Required, MaxLength(5)]
        public string Instance { get; set; }
        [Required]
        public string Url { get; set; }
        [DefaultValue("False")]
        public bool Active { get; set; }

        public ICollection<TransactionEntity> Transactions { get; set; }
    }
}