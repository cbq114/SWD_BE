using System.ComponentModel.DataAnnotations;

namespace Tutor.Domains.Entities
{
    public class Roles
    {
        [Key]

        public int RoleId { get; set; }
        [Required]
        public string? RoleName { get; set; }
        public virtual ICollection<Users> Users { get; set; }
    }
}
