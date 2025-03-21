using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.Message;

namespace Tutor.Infratructures.Interfaces
{
    public interface IMessageRepository
    {
        Task<List<MessageRooms>> GetRoomsByUsername(string username);
        Task<MessageRooms> GetExistingDirectRoom(string currentUsername, string otherUsername);
        Task<List<MessageWithAvatarDTO>> GetMessageHistory(int roomId);
        Task<MessageRooms> CreateRoom(MessageRooms room);
        Task SaveMessage(MessageContents message);
        Task UpdateLastSeen(int roomId, string username);
        Task<int> GetUnreadMessageCount(string username);

        Task MarkMessagesAsRead(int roomId, string username);
        Task<List<string>> GetUsersInRoom(int roomId);

    }
}
