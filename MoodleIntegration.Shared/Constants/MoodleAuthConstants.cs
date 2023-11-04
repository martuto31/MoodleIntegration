using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleIntegration.Shared.Constants
{
    public static class MoodleAuthConstants
    {
        public static string Client_Id = "ClientId1";
        public static string Client_Secret = "92ad170a2d2e41c0dc0fea1d6280a9ce99b4cc885bc04938";
        public static string Grant_Type = "authorization_code";
        public static string Scope = "user_info";
        public static string TokenUrl = "http://localhost/local/oauth/token.php";
        public static string User_Info_Url = "http://localhost/local/oauth/user_info.php";
        public static string Rest_API_Url = "http://localhost/webservice/rest/server.php";
    }
}
