namespace Tutor.Infratructures.Models.Message
{
    public class MessageRoomDTO
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsGroup { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public List<MessageRoomMemberDTO> Members { get; set; }
    }

    public class MessageRoomMemberDTO
    {
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
