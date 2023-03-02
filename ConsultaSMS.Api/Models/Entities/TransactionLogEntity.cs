using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebConsultaSMS.Models.Entities
{
    public class TransactionLogEntity : Entity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid? TransactionId { get; set; }
        public Guid? UserId { get; set; }
        public string TokenOrKey { get; set; }
        public string Host { get; set; }
        public string Status { get; set; }
        public string UserAgent { get; set; }
        public int ResponseCode { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public TimeSpan EleasepTime { get; set; }
        public string RequestMDS { get; set; }
        public string ResponseMDS { get; set; }
        public TimeSpan EleasepTimeMDS { get; set; }

        [ForeignKey("TransactionId")]
        public virtual TransactionEntity Transaction { get; set; } = new TransactionEntity();
        //public UserEntity User { get; set; } = new UserEntity();
    }
}
