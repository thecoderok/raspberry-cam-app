namespace RpiProject.Models
{
    using System.Collections;
    using System.Collections.Generic;

    public interface IPhotoEntryRepository
    {
        void Add(PhotoEntry item);

        IEnumerable<PhotoEntry> GetAll();

        PhotoEntry Remove(long id);
    }
}
