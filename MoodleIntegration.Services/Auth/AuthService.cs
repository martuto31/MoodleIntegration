using MoodleIntegration.Shared.Constants;
using MoodleIntegration.Shared.DTO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MoodleIntegration.Services.Auth
{
    public class AuthService : IAuthService
    {
        public async Task<HttpResponseMessage> GetTokenAsync(HttpClient client, string code)
        {
            // Creates the POST request content for getting the token from moodle
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
            // Creates the POST request content for getting the user info from moodle
            var access_token = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("access_token", moodleToken.access_token)
            });

            // Sends the POST request to the Moodle user info endpoint
            var userInfo = await client.PostAsync(MoodleAuthConstants.User_Info_Url, access_token);

            return userInfo;
        }

        public void SaveUserInfo(UserInfoDTO userInfo)
        {
            // Define the path to your Excel file
            string filePath = @"C:\Users\User\Desktop\UserData1";

            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            using (var workbook = new XSSFWorkbook(fs))
            {
                // Access the worksheet you want to work with (or create it if it doesn't exist)
                ISheet worksheet = workbook.GetSheet("UserData1");
                if (worksheet == null)
                {
                    worksheet = workbook.CreateSheet("UserData1");
                }

                // Find the next empty row
                int rowCount = worksheet.LastRowNum + 1;

                // Create a new row
                var row = worksheet.CreateRow(rowCount);

                // Populate the cells with UserInfoDTO properties
                row.CreateCell(0).SetCellValue(userInfo.id);
                row.CreateCell(1).SetCellValue(userInfo.idnumber);
                row.CreateCell(2).SetCellValue(userInfo.auth);
                row.CreateCell(3).SetCellValue(userInfo.username);
                row.CreateCell(4).SetCellValue(userInfo.firstname);
                row.CreateCell(5).SetCellValue(userInfo.lastname);
                row.CreateCell(6).SetCellValue(userInfo.email);
                row.CreateCell(7).SetCellValue(userInfo.lang);
                row.CreateCell(8).SetCellValue(userInfo.country);
                row.CreateCell(9).SetCellValue(userInfo.phone1);
                row.CreateCell(10).SetCellValue(userInfo.address);
                row.CreateCell(11).SetCellValue(userInfo.description);

                // Save the Excel file
                workbook.Write(fs);
            }
        }
    }
}
