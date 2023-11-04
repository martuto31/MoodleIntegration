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
        Task<HttpResponseMessage> RetrieveMoodleCohorts(HttpClient client, string jwt);
        Task<HttpResponseMessage> RetrieveStudentsFromMoodleCohorts(HttpClient client, string cohortId, string jwt);
        Task AddStudentsToUpdateCohortCSV();
        Task AddStudentsTODeleteFromCohortsCSV();
        Task DeleteStudentsFromMoodleCohorts();
    }
}
