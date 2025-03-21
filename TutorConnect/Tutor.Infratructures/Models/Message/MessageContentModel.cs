using Tutor.Domains.Enums;

namespace Tutor.Infratructures.Models.Message
{
    public class MessageWithAvatarDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime DateSent { get; set; }
        public MessageType MessageType { get; set; }
        public int MessageRoomId { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
    }
}
