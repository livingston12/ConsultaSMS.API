using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebConsultaSMS.Models.Entities
{
    public class RoleTransactionEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int RolId { get; set; }
        [Required]
        public Guid TransactionId { get; set; }
        [Required]
        public bool Active { get; set; }
        [ForeignKey("RolId")]
        public RolEntity Rol { get; set; }
        [ForeignKey("TransactionId")]
        public TransactionEntity Transaction { get; set; }
    }
}