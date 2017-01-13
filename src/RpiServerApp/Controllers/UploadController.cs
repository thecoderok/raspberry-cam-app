namespace RpiServerApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;
    using WebStuff;

    public class UploadController : Controller
    {
        [Route("files")]
        [HttpPost]
        [ServiceFilter(typeof(ValidateMimeMultipartContentFilter))]
        public async Task<IActionResult> UploadFiles(FileDescriptionShort fileDescriptionShort)
        {
            var names = new List<string>();
            var contentTypes = new List<string>();
            if (ModelState.IsValid)
            {
                // http://www.mikesdotnetting.com/article/288/asp-net-5-uploading-files-with-asp-net-mvc-6
                // http://dotnetthoughts.net/file-upload-in-asp-net-5-and-mvc-6/
                foreach (var file in fileDescriptionShort.File)
                {
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        contentTypes.Add(file.ContentType);
                        names.Add(fileName);

                        // Extension method update RC2 has removed this 
                        await file.SaveAsAsync(Path.Combine(_optionsApplicationConfiguration.Value.ServerUploadFolder, fileName));
                    }
                }
            }

            var files = new FileResult
            {
                FileNames = names,
                ContentTypes = contentTypes,
                Description = fileDescriptionShort.Description,
                CreatedTimestamp = DateTime.UtcNow,
                UpdatedTimestamp = DateTime.UtcNow,
            };

            _fileRepository.AddFileDescriptions(files);

            return RedirectToAction("ViewAllFiles", "FileClient");
        }
    }
}
