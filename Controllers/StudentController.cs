using Microsoft.AspNetCore.Mvc;
using SmartPlacement.Models;

namespace SmartPlacement.Controllers
{
    public class StudentController : Controller
    {
        private readonly SmartPlacementContext _context;
        private readonly IWebHostEnvironment _environment;

        public StudentController(
            SmartPlacementContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        // =========================
        // LOGIN - GET
        // =========================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        // =========================
        // LOGIN - POST
        // =========================

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error =
                    "Please enter email and password.";

                return View();
            }

            var student = _context.Students
                .FirstOrDefault(s => s.Email == email);

            if (student != null &&
                student.Password == password)
            {
                HttpContext.Session.SetString(
                    "UserRole",
                    "Student");

                HttpContext.Session.SetString(
                    "UserEmail",
                    student.Email);

                HttpContext.Session.SetString(
                    "StudentId",
                    student.Id.ToString());

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error =
                "Invalid email or password.";

            return View();
        }


        // =========================
        // REGISTER - GET
        // =========================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        // =========================
        // REGISTER - POST
        // =========================

        [HttpPost]
        public IActionResult Register(
            string fullName,
            string email,
            string phone,
            string course,
            string college,
            string password)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(course) ||
                string.IsNullOrWhiteSpace(college) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error =
                    "Please fill all required fields.";

                return View();
            }

            if (phone.Length != 10 ||
                !phone.All(char.IsDigit))
            {
                ViewBag.Error =
                    "Please enter a valid 10-digit phone number.";

                return View();
            }

            var existingStudent = _context.Students
                .FirstOrDefault(s => s.Email == email);

            if (existingStudent != null)
            {
                ViewBag.Error =
                    "An account with this email already exists.";

                return View();
            }

            var student = new Student
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                Course = course,
                College = college,
                Password = password,
                ResumeFileName = "Not Uploaded"
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            TempData["RegisterMessage"] =
                "Account created successfully! Please login. 🎉";

            return RedirectToAction("Login");
        }


        // =========================
        // DASHBOARD
        // =========================

        [HttpGet]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Student")
            {
                return RedirectToAction("Login");
            }

            var loggedInEmail =
                HttpContext.Session.GetString("UserEmail");

            var student = _context.Students
                .FirstOrDefault(s => s.Email == loggedInEmail);

            if (student == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            int studentId = student.Id;

            ViewBag.TotalApplications =
                _context.Applications
                    .Count(a => a.StudentId == studentId);

            ViewBag.ShortlistedApplications =
                _context.Applications
                    .Count(a =>
                        a.StudentId == studentId &&
                        a.Status == "Shortlisted");

            ViewBag.JobsAvailable =
                _context.Jobs.Count();

            ViewBag.ProfileComplete = "100%";

            return View();
        }


        // =========================
        // JOBS
        // =========================

        [HttpGet]
        public IActionResult Jobs()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Student")
            {
                return RedirectToAction("Login");
            }

            var jobs = _context.Jobs
                .OrderByDescending(j => j.Id)
                .ToList();

            return View(jobs);
        }


        // =========================
        // JOB DETAILS
        // =========================

        [HttpGet]
        public IActionResult JobDetails(int jobId)
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Student")
            {
                return RedirectToAction("Login");
            }

            var job = _context.Jobs
                .FirstOrDefault(j => j.Id == jobId);

            if (job == null)
            {
                return RedirectToAction("Jobs");
            }

            return View(job);
        }


        // =========================
        // APPLY - GET
        // =========================

        [HttpGet]
        public IActionResult Apply(int jobId)
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Student")
            {
                return RedirectToAction("Login");
            }

            var job = _context.Jobs
                .FirstOrDefault(j => j.Id == jobId);

            if (job == null)
            {
                return RedirectToAction("Jobs");
            }

            ViewBag.JobId = job.Id;
            ViewBag.JobTitle = job.JobTitle;
            ViewBag.CompanyName = job.CompanyName;

            return View();
        }


        // =========================
        // APPLY - POST
        // =========================

        [HttpPost]
        public async Task<IActionResult> Apply(
            int jobId,
            string fullName,
            string email,
            string phone,
            IFormFile resume)
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Student")
            {
                return RedirectToAction("Login");
            }

            var job = _context.Jobs
                .FirstOrDefault(j => j.Id == jobId);

            if (job == null)
            {
                return RedirectToAction("Jobs");
            }


            // =========================
            // FIND LOGGED-IN STUDENT
            // =========================

            var loggedInEmail =
                HttpContext.Session.GetString("UserEmail");

            var student = _context.Students
                .FirstOrDefault(s => s.Email == loggedInEmail);

            if (student == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }


            // =========================
            // REQUIRED FIELDS
            // =========================

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone))
            {
                ViewBag.Error =
                    "Please fill all required fields.";

                ViewBag.JobId = job.Id;
                ViewBag.JobTitle = job.JobTitle;
                ViewBag.CompanyName = job.CompanyName;

                return View();
            }


            // =========================
            // PHONE VALIDATION
            // =========================

            if (phone.Length != 10 ||
                !phone.All(char.IsDigit))
            {
                ViewBag.Error =
                    "Please enter a valid 10-digit phone number.";

                ViewBag.JobId = job.Id;
                ViewBag.JobTitle = job.JobTitle;
                ViewBag.CompanyName = job.CompanyName;

                return View();
            }


            // =========================
            // RESUME REQUIRED
            // =========================

            if (resume == null ||
                resume.Length == 0)
            {
                ViewBag.Error =
                    "Please upload your resume.";

                ViewBag.JobId = job.Id;
                ViewBag.JobTitle = job.JobTitle;
                ViewBag.CompanyName = job.CompanyName;

                return View();
            }


            // =========================
            // RESUME TYPE
            // =========================

            var extension =
                Path.GetExtension(resume.FileName)
                    .ToLower();

            if (extension != ".pdf" &&
                extension != ".doc" &&
                extension != ".docx")
            {
                ViewBag.Error =
                    "Please upload a PDF, DOC or DOCX resume.";

                ViewBag.JobId = job.Id;
                ViewBag.JobTitle = job.JobTitle;
                ViewBag.CompanyName = job.CompanyName;

                return View();
            }


            // =========================
            // DUPLICATE APPLICATION
            // =========================

            var alreadyApplied =
                _context.Applications
                    .Any(a =>
                        a.StudentId == student.Id &&
                        a.JobId == job.Id);

            if (alreadyApplied)
            {
                TempData["SuccessMessage"] =
                    "You have already applied for this job.";

                return RedirectToAction(
                    "Apply",
                    new { jobId = job.Id });
            }


            // =========================
            // SAVE RESUME
            // =========================

            var resumesFolder =
                Path.Combine(
                    _environment.WebRootPath,
                    "resumes");

            if (!Directory.Exists(resumesFolder))
            {
                Directory.CreateDirectory(
                    resumesFolder);
            }


            var fileName =
                Guid.NewGuid().ToString()
                + extension;

            var filePath =
                Path.Combine(
                    resumesFolder,
                    fileName);


            using (var stream =
                new FileStream(
                    filePath,
                    FileMode.Create))
            {
                await resume.CopyToAsync(stream);
            }


            // =========================
            // CREATE APPLICATION
            // =========================

            var application = new Application
            {
                StudentId = student.Id,

                JobId = job.Id,

                AppliedDate =
                    DateTime.Now.ToString(
                        "dd-MM-yyyy HH:mm"),

                Status = "Applied",

                StudentName = fullName,

                JobTitle = job.JobTitle,

                CompanyName = job.CompanyName,

                ResumeFileName = fileName
            };


            _context.Applications.Add(
                application);

            _context.SaveChanges();


            TempData["SuccessMessage"] =
                "Application submitted successfully! 🎉";


            return RedirectToAction(
                "Applications");
        }


        // =========================
        // MY APPLICATIONS
        // =========================

        [HttpGet]
        public IActionResult Applications()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Student")
            {
                return RedirectToAction("Login");
            }

            var loggedInEmail =
                HttpContext.Session.GetString("UserEmail");

            var student = _context.Students
                .FirstOrDefault(s =>
                    s.Email == loggedInEmail);

            if (student == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            var applications =
                _context.Applications
                    .Where(a =>
                        a.StudentId == student.Id)
                    .OrderByDescending(a => a.Id)
                    .ToList();

            return View(applications);
        }


        // =========================
        // PROFILE - GET
        // =========================

        [HttpGet]
        [Route("Student/Profile")]
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Student")
            {
                return RedirectToAction("Login");
            }

            var loggedInEmail =
                HttpContext.Session.GetString("UserEmail");

            var student = _context.Students
                .FirstOrDefault(s =>
                    s.Email == loggedInEmail);

            if (student == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return View(student);
        }


        // =========================
        // PROFILE - POST / UPDATE
        // =========================

        [HttpPost]
        [Route("Student/Profile")]
        public async Task<IActionResult> Profile(
            Student updatedStudent,
            IFormFile? resume)
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Student")
            {
                return RedirectToAction("Login");
            }

            var loggedInEmail =
                HttpContext.Session.GetString("UserEmail");

            var student = _context.Students
                .FirstOrDefault(s =>
                    s.Email == loggedInEmail);

            if (student == null)
            {
                return RedirectToAction("Login");
            }

            student.FullName =
                updatedStudent.FullName;

            student.Email =
                updatedStudent.Email;

            student.Phone =
                updatedStudent.Phone;

            student.College =
                updatedStudent.College;

            student.Course =
                updatedStudent.Course;


            // =========================
            // RESUME UPLOAD
            // =========================

            if (resume != null &&
                resume.Length > 0)
            {
                var extension =
                    Path.GetExtension(
                        resume.FileName)
                        .ToLower();

                var allowedExtensions =
                    new[]
                    {
                        ".pdf",
                        ".doc",
                        ".docx"
                    };

                if (!allowedExtensions
                    .Contains(extension))
                {
                    TempData["ProfileMessage"] =
                        "Only PDF, DOC and DOCX files are allowed.";

                    return RedirectToAction(
                        "Profile");
                }


                var uploadsFolder =
                    Path.Combine(
                        _environment.WebRootPath,
                        "resumes");


                if (!Directory.Exists(
                    uploadsFolder))
                {
                    Directory.CreateDirectory(
                        uploadsFolder);
                }


                var fileName =
                    Guid.NewGuid().ToString()
                    + extension;

                var filePath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);


                using (var stream =
                    new FileStream(
                        filePath,
                        FileMode.Create))
                {
                    await resume.CopyToAsync(
                        stream);
                }


                student.ResumeFileName =
                    fileName;
            }


            if (string.IsNullOrWhiteSpace(
                student.ResumeFileName))
            {
                student.ResumeFileName =
                    "Not Uploaded";
            }


            _context.SaveChanges();


            HttpContext.Session.SetString(
                "UserEmail",
                student.Email);


            TempData["ProfileMessage"] =
                "Profile updated successfully! 🎉";


            return RedirectToAction(
                "Profile");
        }


        // =========================
        // EDIT PROFILE - GET
        // =========================

        [HttpGet]
        public IActionResult EditProfile()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Student")
            {
                return RedirectToAction("Login");
            }

            var loggedInEmail =
                HttpContext.Session.GetString("UserEmail");

            var student = _context.Students
                .FirstOrDefault(s =>
                    s.Email == loggedInEmail);

            if (student == null)
            {
                return RedirectToAction("Login");
            }

            return View(student);
        }


        // =========================
        // EDIT PROFILE - POST
        // =========================

        [HttpPost]
        public async Task<IActionResult> EditProfile(
            string fullName,
            string email,
            string phone,
            string course,
            string college,
            IFormFile? resume)
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Student")
            {
                return RedirectToAction("Login");
            }


            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(course) ||
                string.IsNullOrWhiteSpace(college))
            {
                ViewBag.Error =
                    "Please fill all required fields.";

                return View(new Student
                {
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    Course = course,
                    College = college
                });
            }


            if (phone.Length != 10 ||
                !phone.All(char.IsDigit))
            {
                ViewBag.Error =
                    "Please enter a valid 10-digit phone number.";

                return View(new Student
                {
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    Course = course,
                    College = college
                });
            }


            var loggedInEmail =
                HttpContext.Session.GetString("UserEmail");

            var student = _context.Students
                .FirstOrDefault(s =>
                    s.Email == loggedInEmail);

            if (student == null)
            {
                return RedirectToAction("Login");
            }


            // =========================
            // UPDATE PROFILE
            // =========================

            student.FullName = fullName;

            student.Email = email;

            student.Phone = phone;

            student.Course = course;

            student.College = college;


            // =========================
            // RESUME UPLOAD
            // =========================

            if (resume != null &&
                resume.Length > 0)
            {
                var extension =
                    Path.GetExtension(
                        resume.FileName)
                        .ToLower();

                var allowedExtensions =
                    new[]
                    {
                        ".pdf",
                        ".doc",
                        ".docx"
                    };

                if (!allowedExtensions
                    .Contains(extension))
                {
                    ViewBag.Error =
                        "Only PDF, DOC and DOCX files are allowed.";

                    return View(student);
                }


                var uploadsFolder =
                    Path.Combine(
                        _environment.WebRootPath,
                        "resumes");


                if (!Directory.Exists(
                    uploadsFolder))
                {
                    Directory.CreateDirectory(
                        uploadsFolder);
                }


                var fileName =
                    Guid.NewGuid().ToString()
                    + extension;

                var filePath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);


                using (var stream =
                    new FileStream(
                        filePath,
                        FileMode.Create))
                {
                    await resume.CopyToAsync(
                        stream);
                }


                student.ResumeFileName =
                    fileName;
            }


            if (string.IsNullOrWhiteSpace(
                student.ResumeFileName))
            {
                student.ResumeFileName =
                    "Not Uploaded";
            }


            _context.SaveChanges();


            HttpContext.Session.SetString(
                "UserEmail",
                student.Email);


            TempData["ProfileMessage"] =
                "Profile updated successfully! 🎉";


            return RedirectToAction(
                "EditProfile");
        }


        // =========================
        // LOGOUT
        // =========================

        [HttpGet]
        [Route("Student/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

    }
}