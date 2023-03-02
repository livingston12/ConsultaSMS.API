using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebConsultaSMS.Models.Entities
{
    public class UserEntity : Entity
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MaxLength(75)]
        public string Name { get; set; }
        [Required, MaxLength(150)]
        public string Email { get; set; }
        [Required, MaxLength(25)]
        public string UserName { get; set; }
        [Required, MaxLength(250)]
        public string Password { get; set; }
        [Required]
        public bool Active { get; set; }
        [DataType(DataType.DateTime)]
        public string Pin { get; set; }
        public int? RolId { get; set; }
        [DefaultValue("False")]
        public bool IsEncriptPassword { get; set; }
        [MaxLength(25)]
        public string MDCodeClient { get; set; }

        [ForeignKey("RolId")]
        public RolEntity Rol { get; set; }

        public ICollection<PhoneEntity> Phones { get; set; }
        public ICollection<TransactionLogEntity> TransactionLogs { get; set; }
        public ICollection<UserTransactionEntity> UserTransactions { get; set; }
    }
}