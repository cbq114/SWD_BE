using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Models.LessonModel;
using Tutor.Infratructures.Models.Responses;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [Authorize(Roles = "Tutor")]
        [HttpPost("add-lesson")]
        public async Task<IActionResult> AddLesson([FromForm] CreateLessonModel model, IFormFile file)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _lessonService.AddLesson(model, username, file);
            return Ok(result);
        }
        [Authorize(Roles = "Tutor")]
        [HttpDelete("delete-lesson/{lessonId}")]
        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            var result = await _lessonService.DeleteLesson(lessonId);
            return Ok(result);
        }

        [HttpGet("get-lesson/{lessonId}")]
        public async Task<IActionResult> GetLessonById(int lessonId)
        {
            var result = await _lessonService.GetLessonById(lessonId);
            return Ok(ApiResponse<LessonViewModel>.SuccessResult(result));
        }
        [Authorize(Roles = "Tutor")]
        [HttpGet("get-lesson-by-tutor")]
        public async Task<IActionResult> GetLessonByTutor()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _lessonService.GetLessonByTutor(username);
            return Ok(ApiResponse<List<LessonViewModel>>.SuccessResult(result));
        }
        [HttpGet("get-lesson-follow-tutor/{username}")]
        public async Task<IActionResult> GetLessonByTutor(string username)
        {
            var result = await _lessonService.GetLessonByTutor(username);
            return Ok(ApiResponse<List<LessonViewModel>>.SuccessResult(result));
        }
        [Authorize(Roles = "Tutor")]
        [HttpPut("update-lesson/{lessonId}")]
        public async Task<IActionResult> UpdateLesson([FromForm] CreateLessonModel model, IFormFile file, int lessonId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _lessonService.UpdateLesson(model, username, file, lessonId);
            return Ok(result);
        }
    }
}
