namespace SmartPlacement.Models
{
    public class Application
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int JobId { get; set; }

        public string AppliedDate { get; set; } = "";

        public string Status { get; set; } = "Applied";

        public string StudentName { get; set; } = "";

        public string JobTitle { get; set; } = "";

        public string CompanyName { get; set; } = "";

        public string ResumeFileName { get; set; } = "";
    }
}