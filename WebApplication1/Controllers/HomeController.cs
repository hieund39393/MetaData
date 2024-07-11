using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITelegramService _telegramBot;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, ITelegramService telegramBot)
        {
            _logger = logger;
            _configuration = configuration;
            _telegramBot = telegramBot;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HttpContext.Session.Clear();
            var ip = GetClientIp();
            var location = GetLocationFromIp(ip).Result;
            HttpContext.Session.SetString(Constants.IP, ip);
            HttpContext.Session.SetString(Constants.LOCATION, location);

            return View();
        }

        [HttpPost]
        public IActionResult Privacy()
        {
            return View();
        }

        private string GetClientIp()
        {

            string ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ip = HttpContext.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0];
            }
            else if (HttpContext.Request.Headers.ContainsKey("X-Real-IP"))
            {
                ip = HttpContext.Request.Headers["X-Real-IP"];
            }

            return ip;
        }

        private async Task<string> GetLocationFromIp(string ip)
        {
            string url = $"http://ipinfo.io/{ip}/json";
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var json = JObject.Parse(response);
                var city = json["city"]?.ToString();
                var region = json["region"]?.ToString();
                var country = json["country"]?.ToString();
                var loc = json["loc"]?.ToString(); // Latitude and Longitude

                return $"{city}, {region}, {country} ({loc})";
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
