namespace RpiProject.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Models;
    using RpiServerApp.Data;
    using RpiServerApp.WebStuff;

    [Route("api/[controller]")]
    [Authorize]
    public class PhotoEntryController : Controller
    {
        private const string UploadFolder = "Images";
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<PhotoEntryController> logger;
        private static readonly char[] InvalidFileNameChars;
        private const long MaxFileSizeBytes = 524288; // 0.5MB

        static PhotoEntryController()
        {
            var temp = new List<char>(Path.GetInvalidFileNameChars()) {'+'};
            InvalidFileNameChars = temp.ToArray();
        }

        public PhotoEntryController(
            ILoggerFactory loggerFactory,
            ApplicationDbContext dbContext)
        {
            this.logger = loggerFactory.CreateLogger<PhotoEntryController>();
            this.logger.LogDebug("Created PhotoEntryController instance");
            this.dbContext = dbContext;

            if (!Directory.Exists(UploadFolder))
            {
                this.logger.LogInformation("Creating folder for photo uploads: {0}.", UploadFolder);
                Directory.CreateDirectory(UploadFolder);
            }
        }

        [HttpGet]
        public async Task<IEnumerable<PhotoEntry>> GetAll()
        {
            var recent = await dbContext.WebcamCaptures
                .OrderByDescending(e => e.WhenAdded)
                .Take(200)
                .ToListAsync();

            var result = new List<PhotoEntry>();
            foreach (var webcamCapture in recent)
            {
                var url = "/" + webcamCapture.Location;
                var item = new PhotoEntry(url, webcamCapture.Id, webcamCapture.WhenAdded);
                result.Add(item);
            }
            return result;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateMimeMultipartContentFilter))]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            this.logger.LogDebug("Uploading file");
            if (file == null)
            {
                return BadRequest("Empty file input");
            }

            if (!file.FileName.ToLower().EndsWith(".jpg"))
            {
                return BadRequest("Invalid file extension");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                return BadRequest("File too big");
            }

            var currentDate = DateTime.Today;
            var folderName = SanitizeFileName(currentDate.ToString("yyyy_MMMM_dd"));
            var fullFolder = Path.Combine(UploadFolder, folderName);
            if (!Directory.Exists(fullFolder))
            {
                this.logger.LogInformation("Created dir: {0}.", fullFolder);
                Directory.CreateDirectory(fullFolder);
            }

            string fileName = Path.GetTempFileName();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            fileName += "_" + DateTime.Now.ToString("yyyy_MMMM_dd_HH_mm_ss_tt_zz") + ".jpg";
            fileName = SanitizeFileName(fileName);

            string destination = Path.Combine(fullFolder, fileName);
            logger.LogInformation("Saving file to: {0}.", destination);
            await file.SaveAsAsync(destination);

            var entry = new WebcamCapture()
            {
                WhenAdded = DateTime.Now,
                Location = destination
            };
            dbContext.WebcamCaptures.Add(entry);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            return string.Join("_", fileName.Split(InvalidFileNameChars));
        }
    }
}
