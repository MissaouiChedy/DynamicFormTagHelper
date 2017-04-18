using DynamicFormTagHelper.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new PersonViewModel());
        }

        [HttpPost]
        public IActionResult Index(PersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                return Show(model);
            }
            return View(model);
        }

        public IActionResult Show(PersonViewModel model)
        {
            return View("Show", model);
        }

        [HttpGet]
        public IActionResult DogNames(string typed)
        {
            var names = new List<string>()
            {
                "DaDawg",
                "Dawg",
                "Rex",
                "Kablam",
                "Fenton",
                "Fenta"
            };
            var result = names.Where(n => n.ToUpper().Contains(typed.ToUpper())).ToList();
            return Json(result);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
