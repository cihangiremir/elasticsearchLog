using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            int vize = 40;
            int final = 0;
            var student = new Student();

            try
            {

                _logger.LogInformation("Öğrenci Bilgileri : {@Student} , Gönderilen paremetreler vize: {vize} , final: {final}", student, vize, final);
                var error = vize / final;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Something went wrong.");
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
    public class Student
    {
        public int Id { get; set; } = 123456;
        public string Name { get; set; } = "Emirhan";
        public string Surname { get; set; } = "Cihangir";
    }
}
