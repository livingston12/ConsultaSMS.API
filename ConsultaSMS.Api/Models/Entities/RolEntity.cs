using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebConsultaSMS.Models.Entities
{
    public class RolEntity
    {
        [Key]
        public int RolId { get; set; }
        [Required, MaxLength(25)]
        public string Name { get; set; }
        [MaxLength(150)]
        public string Description { get; set; }
        [Required]
        public bool Active { get; set; }

        public ICollection<UserEntity> Users { get; set; }
        public ICollection<RoleTransactionEntity> RoleTransactions { get; set; }
    }
}