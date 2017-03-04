using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicFormTagHelper.Models;
using Microsoft.AspNetCore.Mvc;

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
                return RedirectToAction("Show", "Home", model);
            }
            return View(model);
        }

        public IActionResult Show(PersonViewModel model)
        {
            return Content(model.ToString());
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
