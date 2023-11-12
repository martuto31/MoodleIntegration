using CsvHelper;
using CsvHelper.Configuration;
using MathNet.Numerics.Distributions;
using MoodleIntegration.Shared.Constants;
using MoodleIntegration.Shared.DTO;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoodleIntegration.Services.Auth
{
    public class CohortManagementService : ICohortManagementService
    {
        public void AddStudentsToUpdateCohortCSV(HttpClient client, string jwt, List<StudentInfoDTO> studentsToAddToMoodle)
        {
            // Assuming studentsToAddToMoodle is not null
            if (studentsToAddToMoodle != null && studentsToAddToMoodle.Any())
            {
                // Specify the path to your CSV file
                string filePath = @"C:\\Users\\User\\Downloads\MoodleUpdate.csv";

                // Create a CSV configuration (optional)
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true, // The first row is the header
                    Encoding = Encoding.UTF8, // Set the encoding
                };

                // Append data to the CSV file
                using (var writer = new CsvWriter(new StreamWriter(filePath, append: true, encoding: Encoding.UTF8), config))
                {
                    writer.WriteRecords(studentsToAddToMoodle);
                }

                // data added
            }
            else
            {
                // no data
            }
        }

        public void AddStudentsToDeleteFromCohortsCSV(HttpClient client, string jwt, List<UserInfoDTO> studentsRemovedFromCohort)
        {
            // Assuming studentsToAddToMoodle is not null
            if (studentsRemovedFromCohort != null && studentsRemovedFromCohort.Any())
            {
                // Specify the path to your CSV file
                string filePath = @"C:\\Users\\User\\Downloads\MoodleDeleteFromCohort.csv";

                // Create a CSV configuration (optional)
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true, // The first row is the header
                    Encoding = Encoding.UTF8, // Set the encoding
                };

                // Append data to the CSV file
                using (var writer = new CsvWriter(new StreamWriter(filePath, append: true, encoding: Encoding.UTF8), config))
                {
                    writer.WriteRecords(studentsRemovedFromCohort);
                }

                // data added
            }
            else
            {
                // no data
            }
        }

        public async Task DeleteStudentsFromMoodleCohort(HttpClient client, string jwt, int cohortId, int studentId)
        {
            throw new NotImplementedException();
        }

        public async Task ExtractStudentsToRemoveOrAddToMoodleAsync(HttpClient client, string jwt)
        {
            var allMoodleCohorts = await GetMoodleCohortsAsync(client, jwt);
            var studentsFromCSVGroupedByCohorts = ExtractStudentDataByCohortsFromCSV();

            // Iterates through every cohort in moodle
            foreach (var moodleCohort in allMoodleCohorts)
            {
                // vzima userIds
                // itererame vseki user i vzimame info za nego i go mahame dolu polse vuv foreacha
                // refactor da chekva za null

                // Takes the student ids from the given moodle cohort
                var studentIdsFromMoodle = await GetStudentsIDsFromMoodleCohortsAsync(client, moodleCohort.id, jwt);

                // If there are no students from the CSV file mathing the current cohort, skip the cohort iteration
                bool studentsFromCSVContainsCohort = studentsFromCSVGroupedByCohorts.ContainsKey(moodleCohort.name);
                if (studentsFromCSVContainsCohort == false)
                {
                    continue;
                }

                // Gets the students from the CSV matching the same cohort
                var studentsFromCsv = studentsFromCSVGroupedByCohorts[moodleCohort.name];

                // List of student ids from moodle, matching the given cohort
                var studentsIdsFromMoodle = studentIdsFromMoodle[0].userids ?? throw new Exception();

                var moodleUpdateData = await GetMoodleUpdateDataAsync(client, jwt, studentsIdsFromMoodle, studentsFromCsv);

                if(moodleUpdateData.studentsToAddToMoodle != null && moodleUpdateData.studentsToAddToMoodle.Any())
                {
                    AddStudentsToUpdateCohortCSV(client, jwt, moodleUpdateData.studentsToAddToMoodle);
                }

                if (moodleUpdateData.studentsToRemoveFromCohort != null && moodleUpdateData.studentsToRemoveFromCohort.Any())
                {
                    AddStudentsToDeleteFromCohortsCSV(client, jwt, moodleUpdateData.studentsToRemoveFromCohort);
                }
            }
        }

        public List<StudentInfoDTO> ExtractStudentDataFromCSV()
        {
            List<StudentInfoDTO> records = new List<StudentInfoDTO>();
            string pathToCSV = @"C:\\Users\\User\\Downloads\cohort_interrupt.csv";

            // Read the CSV file and map it to a list of StudentInfoDTO objects
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true, // The first row is the header
                Encoding = Encoding.UTF8 // Set the encoding
            };

            using (var reader = new StreamReader(pathToCSV))
            using (var csv = new CsvReader(reader, config))
            {
                records = csv.GetRecords<StudentInfoDTO>().ToList();
            }

            return records;
        }

        public async Task<List<MoodleCohortsDTO>> GetMoodleCohortsAsync(HttpClient client, string jwt)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("wstoken", jwt),
                    new KeyValuePair<string, string>("wsfunction", "core_cohort_get_cohorts"),
                    new KeyValuePair<string, string>("moodlewsrestformat", "json")
            });

            var response = await client.PostAsync(MoodleAuthConstants.Rest_API_Url, content);
            List<MoodleCohortsDTO> allMoodleCohorts = new List<MoodleCohortsDTO>();

            if (response != null && response.IsSuccessStatusCode)
            {
                var getMoodleCohortsResponseContent = await response.Content.ReadAsStringAsync();
                allMoodleCohorts = JsonSerializer.Deserialize<List<MoodleCohortsDTO>>(getMoodleCohortsResponseContent);
            }

            return allMoodleCohorts;
        }

        public async Task<List<MoodleCohortUsersDTO>> GetStudentsIDsFromMoodleCohortsAsync(HttpClient client, int cohortId, string jwt)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("wstoken", jwt),
                    new KeyValuePair<string, string>("wsfunction", "core_cohort_get_cohort_members"),
                    new KeyValuePair<string, string>("moodlewsrestformat", "json"),
                    new KeyValuePair<string, string>("cohortids[]", cohortId.ToString())
            });

            var responseMessage = await client.PostAsync(MoodleAuthConstants.Rest_API_Url, content);
            var response = await responseMessage.Content.ReadAsStringAsync();
            var studentsFromMoodle = JsonSerializer.Deserialize<List<MoodleCohortUsersDTO>>(response);

            return studentsFromMoodle;
        }

        public async Task<UserInfoDTO?> GetUserByIdAsync(HttpClient client, int userId, string jwt)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("wstoken", jwt),
                    new KeyValuePair<string, string>("wsfunction", "core_user_get_users_by_field"),
                    new KeyValuePair<string, string>("moodlewsrestformat", "json"),
                    new KeyValuePair<string, string>("field", "id"),
                    new KeyValuePair<string, string>("values[0]", userId.ToString())
            });

            var responseMessage = await client.PostAsync(MoodleAuthConstants.Rest_API_Url, content);
            if (responseMessage.IsSuccessStatusCode)
            {
                var response = await responseMessage.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<List<UserInfoDTO>>(response);

                return userInfo[0];
            }
            else
            {
                return null;
            }
        }

        private Dictionary<string, List<StudentInfoDTO>> ExtractStudentDataByCohortsFromCSV()
        {
            var unsortedRecords = this.ExtractStudentDataFromCSV();

            // Groups the students by cohortId and sets the dictionary key to the cohortId
            var groupedRecords = unsortedRecords.GroupBy(s => s.Cohort1).ToDictionary(g => g.Key, g => g.ToList());

            return groupedRecords;
        }

        private async Task<DataToUpdateMoodleDTO> GetMoodleUpdateDataAsync(HttpClient client, string jwt, List<int> studentsIdsFromMoodle, List<StudentInfoDTO> studentsFromCsv)
        {
            // If there is no cohort in moodle, add it?
            // Keeps a copy of the id of the students from moodle and removes a student from the list if he is present in the moodle CSV
            // If the list is not empty, the users inside are to be removed
            var trackStudentsToRemoveFromMoodle = studentsIdsFromMoodle.ToList();

            // Removes the students that are already in moodle from the lists of students from moodle and csv
            // If the student is in moodle but not in the csv -> delete student from the cohort
            foreach (var moodleStudentId in studentsIdsFromMoodle)
            {
                var moodleStudent = await GetUserByIdAsync(client, moodleStudentId, jwt);

                var isStudentAlreadyInMoodle = studentsFromCsv.FirstOrDefault(x => x.Username == moodleStudent.username || x.Email == moodleStudent.email);
                if (isStudentAlreadyInMoodle != null)
                {
                    // If the student from CSV is already in Moodle, remove the student from the current list of moodle and csv students
                    studentsFromCsv.Remove(isStudentAlreadyInMoodle);
                    trackStudentsToRemoveFromMoodle.Remove(moodleStudentId);
                }
            }

            List<UserInfoDTO> studentsToRemoveFromCohort = new List<UserInfoDTO>();

            // Gets the students that will be deleted from cohort
            foreach (var moodleStudentId in trackStudentsToRemoveFromMoodle)
            {
                var student = await GetUserByIdAsync(client, moodleStudentId, jwt);
                studentsToRemoveFromCohort.Add(student);
            }

            DataToUpdateMoodleDTO dataToUpdateMoodleDTO = new DataToUpdateMoodleDTO();
            dataToUpdateMoodleDTO.studentsToRemoveFromCohort = studentsToRemoveFromCohort;
            dataToUpdateMoodleDTO.studentsToAddToMoodle = studentsFromCsv;

            // Returns the list of students for upload and remove
            return dataToUpdateMoodleDTO;
        }
    }
}
