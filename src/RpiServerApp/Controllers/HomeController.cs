using System;
using Microsoft.AspNetCore.Mvc;

namespace RpiServerApp.Controllers
{
    using Microsoft.AspNetCore.Authorization;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ShowLatest(string u, string p)
        {
            if (string.IsNullOrWhiteSpace(u) || string.IsNullOrWhiteSpace(p))
            {
                throw new ArgumentNullException();
            }

            return null;
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
