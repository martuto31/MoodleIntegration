using MoodleIntegration.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleIntegration.Services.Auth
{
    public interface IAuthService
    {
        Task<HttpResponseMessage> GetTokenAsync(HttpClient client, string code);
        Task<HttpResponseMessage> GetUserInfoAsync(HttpClient client, MoodleTokenDTO moodleToken);
        void SaveUserInfo(UserInfoDTO userInfo);
    }
}
