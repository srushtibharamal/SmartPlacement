namespace SmartPlacement.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Password { get; set; } = "";

        public string Phone { get; set; } = "";

        public string Course { get; set; } = "";

        public string College { get; set; } = "";

        public string ResumeFileName { get; set; } = "";
    }
}