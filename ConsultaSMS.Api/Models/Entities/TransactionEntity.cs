using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebConsultaSMS.Models.Entities
{
    public class TransactionEntity : Entity
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MaxLength(75)]
        public string Name { get; set; }
        [Required, MaxLength(150)]
        public string Description { get; set; }
        [Required, MaxLength(50)]
        public string Channel { get; set; }
        [Required, MaxLength(15)]
        public string TrnCode { get; set; }
        [Required, ForeignKey("Mediator")]
        public Guid MediatorId { get; set; }
        [DefaultValue("False")]
        public bool Active { get; set; }

        public MediatorEntity Mediator { get; set; }
        public ICollection<TransactionLogEntity> TransactionLogs { get; set; }
        public ICollection<UserTransactionEntity> UserTransactions { get; set; }
        public ICollection<RoleTransactionEntity> RoleTransactions { get; set; }

    }
}