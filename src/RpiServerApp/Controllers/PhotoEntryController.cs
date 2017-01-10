namespace RpiProject.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [Route("api/[controller]")]
    public class PhotoEntryController : Controller
    {
        private readonly IPhotoEntryRepository photoEntryRepository;

        public PhotoEntryController(IPhotoEntryRepository photoEntryRepository)
        {
            this.photoEntryRepository = photoEntryRepository;
        }

        [HttpGet]
        public IEnumerable<PhotoEntry> GetAll()
        {
            return photoEntryRepository.GetAll();
        }
    }
}