using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebConsultaSMS.Models.Entities
{

    public class UserTransactionEntity : Entity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TransactionId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }
        [ForeignKey("TransactionId")]
        public TransactionEntity Transaction { get; set; }

    }
}