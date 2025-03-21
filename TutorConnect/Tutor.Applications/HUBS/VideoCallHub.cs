using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Tutor.Applications.Interfaces;

namespace Tutor.Applications.HUBS
{
    [Authorize]
    public class VideoCallHub : Hub
    {
        private readonly IUserService _userService;
        private static readonly Dictionary<string, string> _userConnectionMap = new();

        public VideoCallHub(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(username))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, username);

                    // Track user connection
                    lock (_userConnectionMap)
                    {
                        _userConnectionMap[username] = Context.ConnectionId;
                    }
                }
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                // Log the error
                throw;
            }
        }

        public async Task InitiateVideoCall(int roomId)
        {
            try
            {
                var callerUsername = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(callerUsername))
                {
                    throw new UnauthorizedAccessException("User not authenticated");
                }

                var targetUser = await _userService.GetTargetUserInRoom(roomId, callerUsername);
                if (targetUser == null)
                {
                    throw new HubException("Target user not found in the room");
                }

                var callerUser = await _userService.GetCurrentUser(callerUsername);
                if (callerUser == null)
                {
                    throw new UnauthorizedAccessException("User not found");
                }

                // Send call request to target user
                await Clients.User(targetUser.UserName).SendAsync("IncomingCall", new
                {
                    roomId = roomId,
                    callerUsername = callerUsername,
                    callerAvatar = callerUser.Avatar,
                    callerDisplayName = callerUser.UserName ?? callerUsername
                });
            }
            catch (Exception ex)
            {
                throw new HubException($"Failed to initiate call: {ex.Message}");
            }
        }

        public async Task AcceptCall(int roomId, string callerUsername)
        {
            try
            {
                var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    throw new UnauthorizedAccessException("User not authenticated");
                }

                // Get the target user in the room, which is the opposite of the current user
                var targetUser = await _userService.GetTargetUserInRoom(roomId, username);
                if (targetUser == null)
                {
                    throw new HubException("Target user not found in the room");
                }

                // Notify caller that call was accepted
                await Clients.User(targetUser.UserName).SendAsync("CallAccepted", new
                {
                    roomId = roomId,
                    acceptedBy = username
                });
            }
            catch (Exception ex)
            {
                throw new HubException($"Failed to accept call: {ex.Message}");
            }
        }

        public async Task RejectCall(int roomId, string callerUsername)
        {
            try
            {
                var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    throw new UnauthorizedAccessException("User not authenticated");
                }

                // Get the target user in the room, which is the opposite of the current user
                var targetUser = await _userService.GetTargetUserInRoom(roomId, username);
                if (targetUser == null)
                {
                    throw new HubException("Target user not found in the room");
                }

                await Clients.User(targetUser.UserName).SendAsync("CallRejected", new
                {
                    roomId = roomId,
                    rejectedBy = username
                });
            }
            catch (Exception ex)
            {
                throw new HubException($"Failed to reject call: {ex.Message}");
            }
        }

        public async Task EndCall(int roomId)
        {
            try
            {
                var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    throw new UnauthorizedAccessException("User not authenticated");
                }

                // Get the target user in the room, which is the opposite of the current user
                var targetUser = await _userService.GetTargetUserInRoom(roomId, username);
                if (targetUser == null)
                {
                    throw new HubException("Target user not found in the room");
                }
                await Clients.User(targetUser.UserName).SendAsync("CallEnded", new
                {
                    roomId = roomId,
                    endedBy = username
                });
            }
            catch (Exception ex)
            {
                throw new HubException($"Failed to end call: {ex.Message}");
            }
        }

        // Updated to use a complex object instead of a string
        public async Task SendOffer(OfferData offerData)
        {
            try
            {
                var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    throw new UnauthorizedAccessException("User not authenticated");
                }

                var targetUser = await _userService.GetTargetUserInRoom(offerData.RoomId, username);
                if (targetUser == null)
                {
                    throw new HubException("Target user not found in the room");
                }

                await Clients.User(targetUser.UserName).SendAsync("ReceiveOffer", new
                {
                    fromUsername = username,
                    offer = offerData.Offer
                });
            }
            catch (Exception ex)
            {
                throw new HubException($"Failed to send offer: {ex.Message}");
            }
        }

        // Updated to use a complex object instead of a string
        public async Task SendAnswer(AnswerData answerData)
        {
            try
            {
                var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    throw new UnauthorizedAccessException("User not authenticated");
                }

                var targetUser = await _userService.GetTargetUserInRoom(answerData.RoomId, username);
                if (targetUser == null)
                {
                    throw new HubException("Target user not found in the room");
                }

                await Clients.User(targetUser.UserName).SendAsync("ReceiveAnswer", new
                {
                    fromUsername = username,
                    answer = answerData.Answer
                });
            }
            catch (Exception ex)
            {
                throw new HubException($"Failed to send answer: {ex.Message}");
            }
        }

        // Updated to use a complex object instead of a string
        public async Task SendIceCandidate(IceCandidateData iceCandidateData)
        {
            try
            {
                var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    throw new UnauthorizedAccessException("User not authenticated");
                }

                var targetUser = await _userService.GetTargetUserInRoom(iceCandidateData.RoomId, username);
                if (targetUser == null)
                {
                    throw new HubException("Target user not found in the room");
                }

                await Clients.User(targetUser.UserName).SendAsync("ReceiveIceCandidate", new
                {
                    fromUsername = username,
                    iceCandidate = iceCandidateData.IceCandidate
                });
            }
            catch (Exception ex)
            {
                throw new HubException($"Failed to send ICE candidate: {ex.Message}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(username))
            {
                lock (_userConnectionMap)
                {
                    _userConnectionMap.Remove(username);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }

    // Add these data transfer objects
    public class OfferData
    {
        public int RoomId { get; set; }
        public string Offer { get; set; }
    }

    public class AnswerData
    {
        public int RoomId { get; set; }
        public string Answer { get; set; }
    }

    public class IceCandidateData
    {
        public int RoomId { get; set; }
        public string IceCandidate { get; set; }
    }
}