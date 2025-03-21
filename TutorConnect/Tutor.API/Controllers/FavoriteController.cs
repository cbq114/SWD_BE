using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Models.FavoriteModel;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }
        [HttpGet]
        public async Task<ActionResult<List<FavoriteDetailsModel>>> GetFavoriteInstructors(int pageNumber, int pageSize)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var favoriteInstructors = await _favoriteService.GetFavoriteInstructors(username);
            if (favoriteInstructors == null || !favoriteInstructors.Any())
            {
                return NotFound();
            }
            var PageList = favoriteInstructors.ToPagedList(pageNumber, pageSize);

            return Ok(PageList);
        }
        [HttpPost]
        public async Task<ActionResult> AddFavoriteInstructor(string instructorId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _favoriteService.AddFavoriteInstructor(username, instructorId);
            if (!result)
            {
                return BadRequest("Instructor already added to favorites.");
            }
            return Ok("Instructor added successfully");
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveFavoriteInstructor(string instructorId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _favoriteService.RemoveFavoriteInstructor(username, instructorId);
            if (!result)
            {
                return NotFound("Instructor not found in favorites.");
            }
            return Ok("Instructor delete successfully");
        }
    }
}
