using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor.Domains.Entities
{
    public class MessageRooms
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsGroup { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [ForeignKey("Creator")]
        public string CreatedBy { get; set; }

        public virtual Users Creator { get; set; }
        public virtual ICollection<MessageRoomMember> Members { get; set; }
        public virtual ICollection<MessageContents> Messages { get; set; }
    }
}
