using MoodleIntegration.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleIntegration.Services.Auth
{
    public interface ICohortManagementService
    {
        List<StudentInfoDTO> ExtractStudentDataFromCSV();
        Dictionary<string, List<StudentInfoDTO>> ExtractStudentDataByCohortsFromCSV();
        Task<HttpResponseMessage> RetrieveMoodleCohorts(HttpClient client, string jwt);
        Task<HttpResponseMessage> RetrieveStudentsFromMoodleCohorts(HttpClient client, string cohortId, string jwt);
        Task AddStudentsToUpdateCohortCSV(HttpClient client, string jwt);
        Task AddStudentsTODeleteFromCohortsCSV();
        Task DeleteStudentsFromMoodleCohorts();
    }
}
