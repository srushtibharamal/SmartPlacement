namespace SmartPlacement.Models
{
    public class Job
    {
        public int Id { get; set; }

        public string JobTitle { get; set; } = "";

        public string CompanyName { get; set; } = "";

        public string Location { get; set; } = "";

        public string JobType { get; set; } = "";

        public string Salary { get; set; } = "";

        public string Description { get; set; } = "";

        public string SkillsRequired { get; set; } = "";

        public string PostedDate { get; set; } = "";
    }
}