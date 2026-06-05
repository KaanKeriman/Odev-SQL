using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game_Store.Models
{
    [Table ("Kullanıcı")]
    public class User
    {

        [Key]
        [Column("kullanıcı_id")]
        public short Id { get; set; }


        [Required]
        [MaxLength(50)]

        [Column ("eposta")]
        public string UserName { get; set; }
        [Required]
        [Column ("şifre")]
        public string Password { get; set; }
    }
}
