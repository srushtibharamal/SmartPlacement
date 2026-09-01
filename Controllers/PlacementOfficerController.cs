using Microsoft.AspNetCore.Mvc;
using SmartPlacement.Models;

namespace SmartPlacement.Controllers
{
    public class PlacementOfficerController : Controller
    {
        private readonly SmartPlacementContext _context;

        public PlacementOfficerController(SmartPlacementContext context)
        {
            _context = context;
        }

        // =========================
        // LOGIN - GET
        // =========================

        [HttpGet]
        [Route("PlacementOfficer/Login")]
        public IActionResult Login()
        {
            return View();
        }


        // =========================
        // LOGIN - POST
        // =========================

        [HttpPost]
        [Route("PlacementOfficer/Login")]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Please enter your email.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter your password.";
                return View();
            }

            // Single Placement Officer
            // Any email and password can be used for testing.

            HttpContext.Session.SetString(
                "UserRole",
                "PlacementOfficer");

            return RedirectToAction("Dashboard");
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
    string email,
    string password,
    string confirmPassword)
{
    if (string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(password) ||
        string.IsNullOrWhiteSpace(confirmPassword))
    {
        ViewBag.Error = "Please fill all fields.";
        return View();
    }

    if (password != confirmPassword)
    {
        ViewBag.Error = "Passwords do not match.";
        return View();
    }

    HttpContext.Session.SetString(
        "PlacementOfficerEmail",
        email);

    HttpContext.Session.SetString(
        "PlacementOfficerPassword",
        password);

    TempData["RegisterMessage"] =
        "Account created successfully! Please login. 🎉";

    return RedirectToAction("Login");
}


        // =========================
        // DASHBOARD
        // =========================

        [HttpGet]
        [Route("PlacementOfficer/Dashboard")]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "PlacementOfficer")
            {
                return RedirectToAction("Login");
            }

            ViewBag.StudentCount =
                _context.Students.Count();

            ViewBag.CompanyCount =
                _context.Companies.Count();

            ViewBag.JobCount =
                _context.Jobs.Count();

            ViewBag.ApplicationCount =
                _context.Applications.Count();

            ViewBag.ShortlistedCount =
                _context.Applications
                    .Count(a => a.Status == "Shortlisted");

            ViewBag.RejectedCount =
                _context.Applications
                    .Count(a => a.Status == "Rejected");

            return View();
        }


       // =========================
// STUDENTS
// =========================

[HttpGet]
public IActionResult Students()
{
    if (HttpContext.Session.GetString("UserRole") != "PlacementOfficer")
    {
        return RedirectToAction("Login");
    }

    var students = _context.Students
        .OrderByDescending(s => s.Id)
        .ToList();

    return View(students);
}

        // =========================
        // COMPANIES
        // =========================

        [HttpGet]
        [Route("PlacementOfficer/Companies")]
        public IActionResult Companies()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "PlacementOfficer")
            {
                return RedirectToAction("Login");
            }

            var companies = _context.Companies
                .OrderBy(c => c.CompanyName)
                .ToList();

            return View(companies);
        }


        // =========================
        // JOBS
        // =========================

        [HttpGet]
        [Route("PlacementOfficer/Jobs")]
        public IActionResult Jobs()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "PlacementOfficer")
            {
                return RedirectToAction("Login");
            }

            var jobs = _context.Jobs
                .OrderByDescending(j => j.Id)
                .ToList();

            return View(jobs);
        }


        // =========================
        // APPLICATIONS
        // =========================

        [HttpGet]
        [Route("PlacementOfficer/Applications")]
        public IActionResult Applications()
        {
            if (HttpContext.Session.GetString("UserRole")
                != "PlacementOfficer")
            {
                return RedirectToAction("Login");
            }

            var applications = _context.Applications
                .OrderByDescending(a => a.Id)
                .ToList();

            return View(applications);
        }


        // =========================
        // LOGOUT
        // =========================

        [HttpGet]
        [Route("PlacementOfficer/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}