namespace RpiProject.Models
{
    using System;

    public class PhotoEntry
    {
        public string Url { get; set; }

        public long Id { get; set; }

        public DateTime DateTaken { get; set; }

        public PhotoEntry(string url, long id, DateTime dateTaken)
        {
            Url = url;
            Id = id;
            DateTaken = dateTaken;
        }
    }
}
