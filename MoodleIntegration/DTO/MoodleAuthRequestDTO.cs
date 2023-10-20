namespace MoodleIntegration.DTO
{
    public class MoodleAuthRequestDTO
    {
        public string Code { get; set; }
        public string Client_Id { get; set; }
        public string Client_Secret { get; set; }
        public string Grant_Type { get; set; }
        public string Scope { get; set; }
    }
}
