using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.Message;
using Tutor.Infratructures.Persistence;
using Tutor.Shared.Helper;

namespace Tutor.Infratructures.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly TutorDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(TutorDBContext dbContext, IMapper mapper, ILogger<MessageRepository> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<MessageRooms> CreateRoom(MessageRooms room)
        {
            await _dbContext.MessageRooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();
            return room;
        }

        public async Task<MessageRooms> GetExistingDirectRoom(string currentUsername, string otherUsername)
        {
            return await _dbContext.MessageRooms
                                     .Include(r => r.Members)
                                     .Where(r => !r.IsGroup &&
                                                r.Members.Count == 2 &&
                                                r.Members.Any(m => m.Username == currentUsername) &&
                                                r.Members.Any(m => m.Username == otherUsername))
                                     .FirstOrDefaultAsync();
        }

        public async Task<List<MessageWithAvatarDTO>> GetMessageHistory(int roomId)
        {
            var messages = await _dbContext.MessageContents
                .Where(m => m.MessageRoomId == roomId)
                .Include(m => m.User)
                .Select(m => new MessageWithAvatarDTO
                {
                    Id = m.Id,
                    Content = m.Content,
                    DateSent = m.DateSent,
                    MessageType = m.MessageType,
                    MessageRoomId = m.MessageRoomId,
                    Username = m.Username,
                    Avatar = m.User.Avatar
                })
                .ToListAsync();

            return messages;
        }

        public async Task<List<MessageRooms>> GetRoomsByUsername(string username)
        {
            return await _dbContext.MessageRooms
                                    .Include(r => r.Members)
                                        .ThenInclude(m => m.User)
                                    .Include(r => r.Messages
                                        .OrderByDescending(m => m.DateSent)
                                        .Take(1))
                                    .Where(r => r.Members.Any(m => m.Username == username))
                                    .OrderByDescending(r => r.Messages
                                        .OrderByDescending(m => m.DateSent)
                                        .FirstOrDefault().DateSent)
                                    .ToListAsync();
        }

        public Task SaveMessage(MessageContents message)
        {
            message.DateSent = DateTimeHelper.GetVietnamNow();
            var entity = _mapper.Map<MessageContents>(message);
            _dbContext.MessageContents.Add(entity);
            return _dbContext.SaveChangesAsync();
        }

        public async Task UpdateLastSeen(int roomId, string username)
        {
            try
            {
                var member = await _dbContext.MessageRoomMember
                    .FirstOrDefaultAsync(m =>
                        m.MessageRoomId == roomId &&
                        m.Username != username);

                if (member != null)
                {
                    member.LastSeen = DateTimeHelper.GetVietnamNow();
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    _logger.LogWarning(
                        "Attempted to update last seen for non-existent room member. " +
                        "Room ID: {RoomId}, Username: {Username}",
                        roomId,
                        username
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updating last seen for user {Username} in room {RoomId}",
                    username,
                    roomId
                );
                throw;
            }
        }
        public async Task<int> GetUnreadMessageCount(string username)
        {
            return await _dbContext.MessageContents
                .Where(m => m.Username == username && m.MessageType == MessageType.Unread)
                .CountAsync();
        }
        public async Task MarkMessagesAsRead(int roomId, string username)
        {
            var messages = await _dbContext.MessageContents
                .Where(m => m.MessageRoomId == roomId && m.Username != username && m.MessageType == MessageType.Unread)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.MessageType = MessageType.Read;
            }

            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<string>> GetUsersInRoom(int roomId)
        {
            try
            {
                var members = await _dbContext.MessageRoomMember
                    .Where(m => m.MessageRoomId == roomId)
                    .Select(m => m.Username)
                    .ToListAsync();

                return members;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving users for room {RoomId}",
                    roomId
                );
                throw;
            }
        }

    }
}
