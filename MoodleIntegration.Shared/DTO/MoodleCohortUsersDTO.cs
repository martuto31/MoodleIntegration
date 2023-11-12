using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleIntegration.Shared.DTO
{
    public class MoodleCohortUsersDTO
    {
        public int cohortid { get; set; }
        public List<int>? userids { get; set; }
    }
}
