using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Models.Message;
using Tutor.Infratructures.Models.Responses;

namespace Tutor.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public ChatController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        [HttpGet("get-room")]
        public async Task<IActionResult> GetRooms()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rooms = await _messageService.GetUserRooms(username);
            return Ok(rooms);
        }

        [HttpPost("create-room")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var room = await _messageService.CreateNewRoom(request, username);
            return Ok(room);
        }
        [HttpGet("get-message-history/{roomId}")]
        public async Task<IActionResult> GetMessageHistory(int roomId)
        {
            var messages = await _messageService.GetMessageHistory(roomId);
            return Ok(messages);
        }
        [HttpPut("rooms/{roomId}/last-seen")]
        public async Task<IActionResult> UpdateLastSeen(int roomId, [FromBody] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Username is required"));
            }

            try
            {
                await _messageService.UpdateLastSeen(roomId, username);
                return Ok(ApiResponse<object>.SuccessResult(null, "Last seen updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult(ex.Message));
            }
        }
        [HttpGet("unread-message-count")]
        public async Task<IActionResult> GetUnreadMessageCount()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Username is required"));
            }

            try
            {
                var count = await _messageService.GetUnreadMessageCount(username);
                return Ok(ApiResponse<int>.SuccessResult(count, "Unread message count retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult(ex.Message));
            }
        }
        [HttpPut("rooms/{roomId}/mark-messages-read")]
        public async Task<IActionResult> MarkMessagesAsRead(int roomId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Username is required"));
            }

            try
            {
                await _messageService.MarkMessagesAsRead(roomId, username);
                return Ok(ApiResponse<object>.SuccessResult(null, "Messages marked as read successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult(ex.Message));
            }
        }
        [HttpGet("get-room-users/{roomId}")]
        public async Task<IActionResult> GetRoomUsers(int roomId)
        {
            try
            {
                var users = await _messageService.GetUsersInRoom(roomId);
                return Ok(ApiResponse<List<string>>.SuccessResult(users, "Room users retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult(ex.Message));
            }
        }
    }
}