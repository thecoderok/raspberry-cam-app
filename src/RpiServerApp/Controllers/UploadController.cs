namespace RpiServerApp.Controllers
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using WebStuff;

    public class UploadController : Controller
    {
        private const string UploadFolder = "Images";

        public UploadController()
        {
            if (!Directory.Exists(UploadFolder))
            {
                Directory.CreateDirectory(UploadFolder);
            }
        }

        [Route("api/[controller]")]
        [HttpPost]
        [ServiceFilter(typeof(ValidateMimeMultipartContentFilter))]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            
            await file.SaveAsAsync(UploadFolder + "/test.png");

            return Ok();
        }
    }
}
