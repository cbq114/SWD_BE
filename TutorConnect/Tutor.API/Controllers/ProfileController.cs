using Microsoft.AspNetCore.Mvc;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Models.Responses;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _profileService.GetAllCountriesAsync();
            return Ok(ApiResponse<List<string>>.SuccessResult(countries));
        }
    }
}
