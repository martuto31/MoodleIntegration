using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoodleIntegration.Constants;
using MoodleIntegration.DTO;
using System.Text.Json;

namespace MoodleIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("Callback")]
        public async Task<IActionResult> HandleMoodleCallback(string code)
        {
            MoodleAuthRequestDTO AuthRquestDTO = new MoodleAuthRequestDTO
            {
                Code = code,
                Client_Id = MoodleAuthConstants.Client_Id,
                Client_Secret = MoodleAuthConstants.Client_Secret,
                Grant_Type = MoodleAuthConstants.Grant_Type,
                Scope = MoodleAuthConstants.Scope
            };

            using (var client = new HttpClient())
            {
                // Define the URL of the Moodle token endpoint
                string tokenUrl = "http://localhost/local/oauth/token.php";
                string user_info_url = "http://localhost/local/oauth/user_info.php";

                // Create the POST request content with the parameters
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", AuthRquestDTO.Code),
                    new KeyValuePair<string, string>("client_id", AuthRquestDTO.Client_Id),
                    new KeyValuePair<string, string>("client_secret", AuthRquestDTO.Client_Secret),
                    new KeyValuePair<string, string>("grant_type", AuthRquestDTO.Grant_Type),
                    new KeyValuePair<string, string>("scope", AuthRquestDTO.Scope)
                });

                // Send the POST request to the Moodle token endpoint
                var response = await client.PostAsync(tokenUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    MoodleTokenDTO moodleToken = JsonSerializer.Deserialize<MoodleTokenDTO>(responseContent);

                    var access_token = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("access_token", moodleToken.Access_Token)
                    });

                    var user_info = await client.PostAsync(user_info_url, access_token);
                }
                else
                {
                    // Handle the error response here
                }
            }

            return Redirect("http://moodledomain.com/local/oauth/index.php");
        }
    }
}
