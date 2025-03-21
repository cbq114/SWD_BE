using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Enums;

namespace Tutor.Domains.Entities
{
    public class MessageContents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime DateSent { get; set; }

        public MessageType MessageType { get; set; }

        [Required]
        public int MessageRoomId { get; set; }

        [Required]
        public string Username { get; set; }

        // Navigation properties
        [ForeignKey("MessageRoomId")]
        public virtual MessageRooms? MessageRoom { get; set; }
        [ForeignKey("Username")]
        public virtual Users? User { get; set; }
    }
}
