using Microsoft.AspNetCore.Mvc;
using MoodleIntegration.Services.Auth;
using MoodleIntegration.Shared.DTO;
using System.Text.Json;

namespace MoodleIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("Callback")]
        public async Task<IActionResult> HandleMoodleCallback(string code)
        {
            using (var client = new HttpClient())
            {
                var response = await _authService.GetTokenAsync(client, code);

                if (response.IsSuccessStatusCode)
                {
                    var AuthResponseContent = await response.Content.ReadAsStringAsync();
                    MoodleTokenDTO moodleToken = JsonSerializer.Deserialize<MoodleTokenDTO>(AuthResponseContent);

                    var UserInfoResponseContent = await _authService.GetUserInfoAsync(client, moodleToken);
                }
                else
                {
                    return BadRequest(response);
                }
            }

            return Redirect("http://moodledomain.com/local/oauth/index.php");
        }
    }
}
