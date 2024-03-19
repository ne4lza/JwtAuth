using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using WebUi.Models;

namespace WebUi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async  Task<IActionResult> Index(string Email , string Password)
        {
            LoginUserDTO loginUserDTO = new LoginUserDTO()
            {
                Email = Email,
                Password = Password
            };
            string jsonData = JsonConvert.SerializeObject(loginUserDTO);
            using (HttpClient httpClient = new HttpClient())
            {
                string url = "https://localhost:7047/api/Auth/login";
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content);
                await Console.Out.WriteLineAsync(response.StatusCode.ToString());
                await Console.Out.WriteLineAsync(response.Content.ToString());
            }
            return View();
        }

        public IActionResult Privacy()
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
