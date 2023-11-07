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
        public Task AddStudentsTODeleteFromCohortsCSV()
        {
            throw new NotImplementedException();
        }

        public async Task AddStudentsToUpdateCohortCSV(HttpClient client, string jwt)
        {
            var getMoodleCohortsResponse = await RetrieveMoodleCohorts(client, jwt);
            List<MoodleCohortsDTO> allMoodleCohorts = new List<MoodleCohortsDTO>();

            if (getMoodleCohortsResponse != null && getMoodleCohortsResponse.IsSuccessStatusCode)
            {
                var getMoodleCohortsResponseContent = await getMoodleCohortsResponse.Content.ReadAsStringAsync();
                allMoodleCohorts = JsonSerializer.Deserialize<List<MoodleCohortsDTO>>(getMoodleCohortsResponseContent);
            }

            foreach (var cohort in allMoodleCohorts)
            {

            }
        }

        public Task DeleteStudentsFromMoodleCohorts()
        {
            throw new NotImplementedException();
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

        public Dictionary<string, List<StudentInfoDTO>> ExtractStudentDataByCohortsFromCSV()
        {
            var unsortedRecords = this.ExtractStudentDataFromCSV();
            // Groups the students by cohortId and sets the dictionary key to the cohortId
            var groupedRecords = unsortedRecords.GroupBy(s => s.Cohort1).ToDictionary(g => g.Key, g => g.ToList());

            return groupedRecords;
        }

        public async Task<HttpResponseMessage> RetrieveMoodleCohorts(HttpClient client, string jwt)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("wstoken", jwt),
                    new KeyValuePair<string, string>("wsfunction", "core_cohort_get_cohorts"),
                    new KeyValuePair<string, string>("moodlewsrestformat", "json")
            });

            var response = await client.PostAsync(MoodleAuthConstants.Rest_API_Url, content);
            return response;
        }

        public async Task<HttpResponseMessage> RetrieveStudentsFromMoodleCohorts(HttpClient client, string cohortId, string jwt)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("wstoken", jwt),
                    new KeyValuePair<string, string>("wsfunction", "core_cohort_get_cohort_members"),
                    new KeyValuePair<string, string>("moodlewsrestformat", "json"),
                    new KeyValuePair<string, string>("cohortids[]", cohortId)
            });

            var response = await client.PostAsync(MoodleAuthConstants.Rest_API_Url, content);
            return response;
        }
    }
}
