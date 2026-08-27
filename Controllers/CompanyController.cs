using Microsoft.AspNetCore.Mvc;
using SmartPlacement.Models;

namespace SmartPlacement.Controllers
{
    public class CompanyController : Controller
    {
        private readonly SmartPlacementContext _context;

        public CompanyController(SmartPlacementContext context)
        {
            _context = context;
        }


        // =========================
        // COMPANY LOGIN - GET
        // =========================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        // =========================
        // COMPANY LOGIN - POST
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

            var company = _context.Companies
                .FirstOrDefault(c => c.Email == email);

            if (company != null &&
                company.Password == password)
            {
                HttpContext.Session.SetString(
                    "UserRole",
                    "Company");

                HttpContext.Session.SetString(
                    "UserEmail",
                    company.Email);

                HttpContext.Session.SetString(
                    "CompanyId",
                    company.Id.ToString());

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error =
                "Invalid email or password.";

            return View();
        }


        // =========================
        // COMPANY REGISTER - GET
        // =========================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        // =========================
        // COMPANY REGISTER - POST
        // =========================

        [HttpPost]
        public IActionResult Register(
            string companyName,
            string email,
            string phone,
            string location,
            string password)
        {
            if (string.IsNullOrWhiteSpace(companyName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(location) ||
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

            var existingCompany = _context.Companies
                .FirstOrDefault(c => c.Email == email);

            if (existingCompany != null)
            {
                ViewBag.Error =
                    "An account with this email already exists.";

                return View();
            }

            var company = new Company
            {
                CompanyName = companyName,
                Email = email,
                Phone = phone,
                Location = location,
                Password = password
            };

            _context.Companies.Add(company);
            _context.SaveChanges();

            TempData["RegisterMessage"] =
                "Company account created successfully! Please login. 🎉";

            return RedirectToAction("Login");
        }


        // =========================
        // COMPANY DASHBOARD
        // =========================

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Company")
            {
                return RedirectToAction("Login");
            }

            int companyId = 0;

            int.TryParse(
                HttpContext.Session.GetString("CompanyId"),
                out companyId);

            var company = _context.Companies
                .FirstOrDefault(c => c.Id == companyId);

            if (company == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.CompanyName =
                company.CompanyName;

            ViewBag.JobCount =
                _context.Jobs
                    .Count(j =>
                        j.CompanyName ==
                        company.CompanyName);

            ViewBag.ApplicationCount =
                _context.Applications
                    .Count(a =>
                        a.CompanyName ==
                        company.CompanyName);

            ViewBag.StudentCount =
                _context.Students.Count();

            return View();
        }


        // =========================
        // POST JOB - GET
        // =========================

        [HttpGet]
        public IActionResult PostJob()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Company")
            {
                return RedirectToAction("Login");
            }

            return View();
        }


        // =========================
        // POST JOB - POST
        // =========================

        [HttpPost]
        public IActionResult PostJob(
            string jobTitle,
            string location,
            string jobType,
            string salary,
            string skillsRequired,
            string description)
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Company")
            {
                return RedirectToAction("Login");
            }

            if (string.IsNullOrWhiteSpace(jobTitle) ||
                string.IsNullOrWhiteSpace(location) ||
                string.IsNullOrWhiteSpace(jobType) ||
                string.IsNullOrWhiteSpace(salary) ||
                string.IsNullOrWhiteSpace(skillsRequired) ||
                string.IsNullOrWhiteSpace(description))
            {
                ViewBag.Error =
                    "Please fill all required fields.";

                return View();
            }

            int companyId = 0;

            int.TryParse(
                HttpContext.Session.GetString("CompanyId"),
                out companyId);

            var company = _context.Companies
                .FirstOrDefault(c => c.Id == companyId);

            if (company == null)
            {
                return RedirectToAction("Login");
            }

            var job = new Job
            {
                JobTitle = jobTitle,
                CompanyName = company.CompanyName,
                Location = location,
                JobType = jobType,
                Salary = salary,
                SkillsRequired = skillsRequired,
                Description = description,
                PostedDate =
                    DateTime.Now.ToString("dd-MM-yyyy")
            };

            _context.Jobs.Add(job);
            _context.SaveChanges();

            TempData["JobMessage"] =
                "Job posted successfully! 🎉";

            return RedirectToAction("Dashboard");
        }


        // =========================
        // POSTED JOBS
        // =========================

        public IActionResult Jobs()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Company")
            {
                return RedirectToAction("Login");
            }

            int companyId = 0;

            int.TryParse(
                HttpContext.Session.GetString("CompanyId"),
                out companyId);

            var company = _context.Companies
                .FirstOrDefault(c => c.Id == companyId);

            if (company == null)
            {
                return RedirectToAction("Login");
            }

            var jobs = _context.Jobs
                .Where(j =>
                    j.CompanyName ==
                    company.CompanyName)
                .OrderByDescending(j => j.Id)
                .ToList();

            return View(jobs);
        }


        // =========================
        // APPLICATIONS
        // =========================

        public IActionResult Applications()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Company")
            {
                return RedirectToAction("Login");
            }

            int companyId = 0;

            int.TryParse(
                HttpContext.Session.GetString("CompanyId"),
                out companyId);

            var company = _context.Companies
                .FirstOrDefault(c => c.Id == companyId);

            if (company == null)
            {
                return RedirectToAction("Login");
            }

            // Get applications using CompanyName
            var applications = _context.Applications
                .Where(a =>
                    a.CompanyName ==
                    company.CompanyName)
                .OrderByDescending(a => a.Id)
                .ToList();

            return View(applications);
        }


        // =========================
        // SHORTLIST APPLICATION
        // =========================

        [HttpPost]
        public IActionResult Shortlist(int applicationId)
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Company")
            {
                return RedirectToAction("Login");
            }

            int companyId = 0;

            int.TryParse(
                HttpContext.Session.GetString("CompanyId"),
                out companyId);

            var company = _context.Companies
                .FirstOrDefault(c => c.Id == companyId);

            if (company == null)
            {
                return RedirectToAction("Login");
            }

            var application = _context.Applications
                .FirstOrDefault(a =>
                    a.Id == applicationId);

            if (application == null)
            {
                return RedirectToAction(
                    "Applications");
            }

            // Check company ownership
            if (application.CompanyName !=
                company.CompanyName)
            {
                return RedirectToAction(
                    "Applications");
            }

            application.Status =
                "Shortlisted";

            _context.SaveChanges();

            return RedirectToAction(
                "Applications");
        }


        // =========================
        // REJECT APPLICATION
        // =========================

        [HttpPost]
        public IActionResult Reject(int applicationId)
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Company")
            {
                return RedirectToAction("Login");
            }

            int companyId = 0;

            int.TryParse(
                HttpContext.Session.GetString("CompanyId"),
                out companyId);

            var company = _context.Companies
                .FirstOrDefault(c => c.Id == companyId);

            if (company == null)
            {
                return RedirectToAction("Login");
            }

            var application = _context.Applications
                .FirstOrDefault(a =>
                    a.Id == applicationId);

            if (application == null)
            {
                return RedirectToAction(
                    "Applications");
            }

            // Check company ownership
            if (application.CompanyName !=
                company.CompanyName)
            {
                return RedirectToAction(
                    "Applications");
            }

            application.Status =
                "Rejected";

            _context.SaveChanges();

            return RedirectToAction(
                "Applications");
        }


        // =========================
        // STUDENTS
        // =========================

        public IActionResult Students()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "Company")
            {
                return RedirectToAction("Login");
            }

            var students = _context.Students
                .OrderBy(s => s.FullName)
                .ToList();

            return View(students);
        }


        // =========================
        // LOGOUT
        // =========================

        [HttpGet]
        [Route("Company/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}