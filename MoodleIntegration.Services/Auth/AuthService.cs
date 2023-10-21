using MoodleIntegration.Shared.Constants;
using MoodleIntegration.Shared.DTO;

namespace MoodleIntegration.Services.Auth
{
    public class AuthService : IAuthService
    {
        public async Task<HttpResponseMessage> GetTokenAsync(HttpClient client, string code)
        {
            // Create the POST request content with the parameters
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", MoodleAuthConstants.Client_Id),
                    new KeyValuePair<string, string>("client_secret", MoodleAuthConstants.Client_Secret),
                    new KeyValuePair<string, string>("grant_type", MoodleAuthConstants.Grant_Type),
                    new KeyValuePair<string, string>("scope", MoodleAuthConstants.Scope)
                });

            // Send the POST request to the Moodle token endpoint
            var response = await client.PostAsync(MoodleAuthConstants.TokenUrl, content);

            return response;
        }

        public async Task<HttpResponseMessage> GetUserInfoAsync(HttpClient client, MoodleTokenDTO moodleToken)
        {
            var access_token = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("access_token", moodleToken.access_token)
            });

            var userInfo = await client.PostAsync(MoodleAuthConstants.User_Info_Url, access_token);
            return userInfo;
        }
    }
}
