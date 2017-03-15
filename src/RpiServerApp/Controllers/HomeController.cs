using System;
using Microsoft.AspNetCore.Mvc;

namespace RpiServerApp.Controllers
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;

    public class HomeController : Controller
    {
        private ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ShowLatest(string u, string p)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            var entries = await this.dbContext.WebcamCaptures.ToListAsync();
            return View(entries);
        }

        public FileResult Image(string file)
        {
            if (!User.Identity.IsAuthenticated)
            {
                throw new Exception("Not authenticated");
            }
            
            HttpContext.Response.ContentType = "image/jpeg";

            if (string.IsNullOrWhiteSpace(file))
            {
                throw new Exception("");
            }

            file = file.Replace(' ', '+');
            string absolutePath = Path.GetFullPath(file);
            if (!System.IO.File.Exists(absolutePath) || !absolutePath.ToLower().EndsWith("jpg") || !absolutePath.Contains("\\Images\\"))
            {
                throw new Exception("");
            }

            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(file), "image/jpeg")
            {
                FileDownloadName = "i.jpg"
            };


            return result;
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
