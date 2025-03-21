
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Shared.Helper;

namespace Tutor.Applications.HUBS
{
    // ChatHub.cs
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private static readonly Dictionary<string, HashSet<string>> _roomConnections = new();

        public ChatHub(IUserService userService, IMessageService messageService)
        {
            _userService = userService;
            _messageService = messageService;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(username))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, username);
                }
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                // Log the error
                throw;
            }
        }

        public async Task JoinRoom(int roomId)
        {
            try
            {
                var roomGroup = $"Room_{roomId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, roomGroup);

                // Track connection in memory
                lock (_roomConnections)
                {
                    if (!_roomConnections.ContainsKey(roomGroup))
                    {
                        _roomConnections[roomGroup] = new HashSet<string>();
                    }
                    _roomConnections[roomGroup].Add(Context.ConnectionId);
                }

                // Update last seen
                var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(username))
                {
                    await _messageService.MarkMessagesAsRead(roomId, username);
                    await _messageService.UpdateLastSeen(roomId, username);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw;
            }
        }

        public async Task LeaveRoom(int roomId)
        {
            try
            {
                var roomGroup = $"Room_{roomId}";
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomGroup);

                // Remove connection tracking
                lock (_roomConnections)
                {
                    if (_roomConnections.ContainsKey(roomGroup))
                    {
                        _roomConnections[roomGroup].Remove(Context.ConnectionId);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw;
            }
        }

        public async Task SendMessage(string content, int roomId)
        {
            try
            {
                var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    throw new UnauthorizedAccessException("User not authenticated");
                }

                var user = await _userService.GetCurrentUser(username);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("User not found");
                }

                // Create and save message
                var message = new MessageContents
                {
                    Content = content,
                    DateSent = DateTimeHelper.GetVietnamNow(),
                    MessageType = MessageType.Unread,
                    MessageRoomId = roomId,
                    Username = username
                };

                await _messageService.SaveMessage(message);

                // Only broadcast to others in the room, not back to sender
                await Clients.OthersInGroup($"Room_{roomId}").SendAsync("ReceiveMessage", new
                {
                    id = message.Id,
                    content = message.Content,
                    dateSent = message.DateSent,
                    username = message.Username,
                    userRole = user.RoleId,
                    avatar = user.Avatar,
                    roomId = roomId
                });
            }
            catch (Exception ex)
            {
                // Log the error
                throw;
            }
        }

        public async Task SendTypingStatus(int roomId, bool isTyping)
        {
            try
            {
                var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(username))
                {
                    await Clients.OthersInGroup($"Room_{roomId}")
                        .SendAsync("UserTyping", username, isTyping);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw;
            }
        }

        public async Task UpdateLastSeen(int roomId)
        {
            try
            {
                var username = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(username))
                {
                    await _messageService.UpdateLastSeen(roomId, username);
                }
            }
            catch (Exception ex)
            {
                throw new HubException($"Failed to update last seen: {ex.Message}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Clean up connection tracking
            lock (_roomConnections)
            {
                foreach (var room in _roomConnections)
                {
                    room.Value.Remove(Context.ConnectionId);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
