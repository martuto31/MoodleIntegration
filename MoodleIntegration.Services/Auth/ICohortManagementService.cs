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
        Task RetrieveStudentsFromMoodleCohorts();
        Task AddStudentsToUpdateCohortCSV();
        Task AddStudentsTODeleteFromCohortsCSV();
        Task DeleteStudentsFromMoodleCohorts();
    }
}
