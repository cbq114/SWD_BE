using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor.Domains.Entities
{
    public class RefreshTokens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RId { get; set; }

        public string? Token { get; set; }

        public string UserName { get; set; }

        public DateTime Expires { get; set; }
        [ForeignKey("UserName")]
        public Users? User { get; set; }

    }
}
