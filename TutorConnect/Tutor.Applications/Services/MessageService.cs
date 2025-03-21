using AutoMapper;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.Message;
using Tutor.Shared.Helper;

namespace Tutor.Applications.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository messageRepository, IUserService userService, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userService = userService;
            _mapper = mapper;
        }
        public async Task<MessageRoomDTO> CreateNewRoom(CreateRoomRequest request, string currentUsername)
        {
            // Lấy currentUser từ username
            var currentUser = await _userService.GetCurrentUser(currentUsername);

            // Lấy instructor từ request
            var instructor = await _userService.GetCurrentUser(request.InstructorName);

            if (instructor == null)
            {
                throw new ArgumentException("Instructor not found");
            }
            var roomName = $"{currentUser.UserName} & {instructor.UserName}";
            var exisRoom = await _messageRepository.GetExistingDirectRoom(currentUser.UserName, instructor.UserName);
            if (exisRoom != null)
            {
                throw new Exception("Room already exists");
            }

            // Tạo phòng mới
            var room = new MessageRooms
            {
                Name = roomName,
                IsGroup = false,
                CreatedBy = currentUser.UserName,
                CreatedAt = DateTimeHelper.GetVietnamNow(),
                Members = new List<MessageRoomMember>
        {
            new MessageRoomMember
            {
                Username = currentUser.UserName,
                IsAdmin = false,
                LastSeen = DateTimeHelper.GetVietnamNow()
            },
            new MessageRoomMember
            {
                Username = instructor.UserName,
                IsAdmin = true,
                LastSeen = DateTimeHelper.GetVietnamNow()
            }
        }
            };

            await _messageRepository.CreateRoom(room);
            return _mapper.Map<MessageRoomDTO>(room);
        }

        public async Task<List<MessageWithAvatarDTO>> GetMessageHistory(int roomId)
        {
            return await _messageRepository.GetMessageHistory(roomId);
        }

        public async Task<List<MessageRoomDTO>> GetUserRooms(string username)
        {
            var rooms = await _messageRepository.GetRoomsByUsername(username);
            return _mapper.Map<List<MessageRoomDTO>>(rooms);
        }

        public Task SaveMessage(MessageContents message)
        {
            return _messageRepository.SaveMessage(message);
        }

        public Task UpdateLastSeen(int roomId, string username)
        {
            return _messageRepository.UpdateLastSeen(roomId, username);
        }
        public async Task<int> GetUnreadMessageCount(string username)
        {
            return await _messageRepository.GetUnreadMessageCount(username);
        }
        public async Task MarkMessagesAsRead(int roomId, string username)
        {
            await _messageRepository.MarkMessagesAsRead(roomId, username);
        }

        public Task<List<string>> GetUsersInRoom(int roomId)
        {
            return _messageRepository.GetUsersInRoom(roomId);
        }
    }
}
