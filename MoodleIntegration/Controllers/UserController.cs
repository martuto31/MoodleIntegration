using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using MoodleIntegration.Services.Auth;
using MoodleIntegration.Shared.DTO;
using System.ComponentModel.Design;
using System.Text.Json;

namespace MoodleIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICohortManagementService _cohortManagementService;

        public UserController(IAuthService authService, ICohortManagementService cohortManagementService)
        {
            _authService = authService;
            _cohortManagementService = cohortManagementService;
        }

        [HttpGet("Callback")]
        public async Task<IActionResult> HandleMoodleAuthCallbackAsync(string code, string discord_id)
        {
            using (var client = new HttpClient())
            {
                // Sends request to moodle for the authentication token
                var AuthResponse = await _authService.GetTokenAsync(client, code);

                if (AuthResponse.IsSuccessStatusCode)
                {
                    // Deserialize the authentication token
                    var AuthResponseContent = await AuthResponse.Content.ReadAsStringAsync();
                    MoodleTokenDTO moodleToken = JsonSerializer.Deserialize<MoodleTokenDTO>(AuthResponseContent);

                    // Sends request to moodle for the authenticated user's info
                    var UserInfoResponse = await _authService.GetUserInfoAsync(client, moodleToken);
                    if(UserInfoResponse.IsSuccessStatusCode)
                    {
                        // Deserialise the user info
                        var UserInfoResponseContent = await UserInfoResponse.Content.ReadAsStringAsync();
                        UserInfoDTO userInfo = JsonSerializer.Deserialize<UserInfoDTO>(UserInfoResponseContent);

                        if(userInfo != null)
                        {
                            _authService.SaveUserInfo(userInfo);
                        }
                        else
                        {
                            return NoContent();
                        }
                    }
                    else
                    {
                        return BadRequest(UserInfoResponse);
                    }
                }
                else
                {
                    return BadRequest(AuthResponse);
                }
            }

            return Redirect("http://moodledomain.com/local/oauth/index.php");
        }

        [HttpGet("ExtractStudentsFromUISS")]
        public ActionResult ExtractStudents()
        {
            _cohortManagementService.ExtractStudentDataFromCSV();

            return Ok();
        }
    }
}
