namespace RpiProject.Models
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class InMemoryPhotoEntryRepository : IPhotoEntryRepository
    {
        private static ConcurrentDictionary<long, PhotoEntry> entries = new ConcurrentDictionary<long, PhotoEntry>();
        private static long counter = 0;

        static InMemoryPhotoEntryRepository()
        {
            entries.TryAdd(1, new PhotoEntry("yo", 1, DateTime.Now));
            entries.TryAdd(2, new PhotoEntry("yoa", 2, DateTime.MaxValue));
            entries.TryAdd(3, new PhotoEntry("yod", 3, DateTime.UtcNow));
            entries.TryAdd(5, new PhotoEntry("yodg", 4, DateTime.Now));
            entries.TryAdd(9, new PhotoEntry("yot", 6, DateTime.Now));
        }

        public void Add(PhotoEntry item)
        {
            entries.TryAdd(Interlocked.Increment(ref counter), item);
        }

        public IEnumerable<PhotoEntry> GetAll()
        {
            return entries.Values;
        }

        public PhotoEntry Remove(long id)
        {
            PhotoEntry result;
            entries.TryRemove(id, out result);
            return result;
        }
    }
}
