using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebConsultaSMS.Models.Entities
{
    public class PhoneEntity : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(16)]
        public string NumDocument { get; set; }

        [Required, MaxLength(20)]
        public string Telephone { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime ExpitarationDate { get; set; }

        private bool _active;
        public bool Active
        {
            get { return ExpitarationDate > DateTime.Now; }
            set { _active = ExpitarationDate > DateTime.Now; }
        }
        public int NumberSmsSent { get; set; } = 0;

        [Required]
        public string Token { get; set; }

        [Required, MaxLength(50)]
        public string MDCodeClient { get; set; }

        [Required]
        public Guid UserdId { get; set; }

        [ForeignKey(nameof(UserdId))]
        public UserEntity User { get; set; }
    }
}
