using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleIntegration.Shared.DTO
{
    public class DataToUpdateMoodleDTO
    {
        public List<UserInfoDTO>? studentsToRemoveFromCohort { get; set; }
        public List<StudentInfoDTO>? studentsToAddToMoodle { get; set; }
    }
}
