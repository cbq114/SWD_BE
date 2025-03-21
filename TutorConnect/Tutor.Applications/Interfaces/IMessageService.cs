using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.Message;

namespace Tutor.Applications.Interfaces
{
    public interface IMessageService
    {
        Task SaveMessage(MessageContents message);
        Task<List<MessageRoomDTO>> GetUserRooms(string username);
        Task<MessageRoomDTO> CreateNewRoom(CreateRoomRequest request, string currentUsername);
        Task UpdateLastSeen(int roomId, string username);
        Task<List<MessageWithAvatarDTO>> GetMessageHistory(int roomId);
        Task<int> GetUnreadMessageCount(string username);
        Task MarkMessagesAsRead(int roomId, string username);
        Task<List<string>> GetUsersInRoom(int roomId);
    }

}
