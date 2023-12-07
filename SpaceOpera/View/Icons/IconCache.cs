using Cardamom;
using Cardamom.Utils.Suppliers.Promises;

namespace SpaceOpera.View.Icons
{
    public class IconCache
    {
        class CacheEntry
        {
            public int Count { get; set; }
            public IPromise<IconImage> Image { get; }

            public CacheEntry(IPromise<IconImage> image)
            {
                Count = 1;
                Image = image;
            }
        }

        private readonly Dictionary<CompositeKey<object, IconResolution>, CacheEntry> _cache = new();
        private List<IPromise<IconImage>> _toDispose = new();

        public bool TryGetTexture(CompositeKey<object, IconResolution> key, out IPromise<IconImage>? image)
        {
            bool result = _cache.TryGetValue(key, out var entry);
            if (entry != null)
            {
                entry.Count++;
            }
            image = entry?.Image;
            return result;
        }

        public void Put(CompositeKey<object, IconResolution> key, IPromise<IconImage> image)
        {
            _cache.Add(key, new CacheEntry(image));
        }

        public void Return(CompositeKey<object, IconResolution> key)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                entry.Count--;
                if (entry.Count < 1)
                {
                    _toDispose.Add(entry.Image);
                    _cache.Remove(key);
                }
            }
        }

        private void CheckDispose()
        {
            var newToDispose = new List<IPromise<IconImage>>();
            foreach (var entry in _toDispose)
            {
                if (entry.HasValue())
                {
                    entry.Get().Texture.Dispose();
                }
                else
                {
                    newToDispose.Add(entry);
                }
            }
            _toDispose = newToDispose;
        }
    }
}
