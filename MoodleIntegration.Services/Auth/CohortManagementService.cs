using CsvHelper;
using CsvHelper.Configuration;
using MathNet.Numerics.Distributions;
using MoodleIntegration.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleIntegration.Services.Auth
{
    public class CohortManagementService : ICohortManagementService
    {
        public Task AddStudentsTODeleteFromCohortsCSV()
        {
            throw new NotImplementedException();
        }

        public Task AddStudentsToUpdateCohortCSV()
        {
            throw new NotImplementedException();
        }

        public Task DeleteStudentsFromMoodleCohorts()
        {
            throw new NotImplementedException();
        }

        public List<StudentInfoDTO> ExtractStudentDataFromCSV()
        {
            List<StudentInfoDTO> records = new List<StudentInfoDTO>();
            string pathToCSV = @"C:\\Users\\User\\Downloads\first_import.csv";

            // Read the CSV file and map it to a list of StudentInfoDTO objects
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t", // Set the delimiter to tab
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

        public Task RetrieveStudentsFromMoodleCohorts()
        {
            throw new NotImplementedException();
        }
    }
}
