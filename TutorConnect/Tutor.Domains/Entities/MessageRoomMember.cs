using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor.Domains.Entities
{
    public class MessageRoomMember
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public int MessageRoomId { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime LastSeen { get; set; }

        [ForeignKey("Username")]
        public virtual Users User { get; set; }
        [ForeignKey("MessageRoomId")]
        public virtual MessageRooms MessageRoom { get; set; }
    }
}
