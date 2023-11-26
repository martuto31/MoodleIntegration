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
        Task<List<MoodleCohortsDTO>> GetMoodleCohortsAsync(HttpClient client, string jwt);
        Task<List<MoodleCohortUsersDTO>> GetStudentsIDsFromMoodleCohortsAsync(HttpClient client, int cohortId, string jwt);
        Task ExtractStudentsToRemoveOrAddToMoodleAsync(HttpClient client, string jwt);
        Task DeleteStudentsFromMoodleCohort(HttpClient client, string jwt, List<UserInfoDTO> studentsRemovedFromCohort, int cohortId);
        Task AddStudentsToDeleteFromCohortsCSV(HttpClient client, string jwt, List<UserInfoDTO> studentsRemovedFromCohort, int cohortId);
        void AddStudentsToUpdateCohortCSV(HttpClient client, string jwt, List<StudentInfoDTO> studentsToAddToMoodle);
    }
}
