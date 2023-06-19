using Cardamom;
using Cardamom.Graphics;

namespace SpaceOpera.View.Icons
{
    public class IconCache
    {
        class CacheEntry
        {
            public int Count { get; set; }
            public Texture Texture { get; }

            public CacheEntry(Texture texture)
            {
                Count = 1;
                Texture = texture;
            }
        }

        private readonly Dictionary<CompositeKey<object, IconResolution>, CacheEntry> _cache = new();

        public bool TryGetTexture(CompositeKey<object, IconResolution> key, out Texture? texture)
        {
            bool result = _cache.TryGetValue(key, out var entry);
            if (entry != null)
            {
                entry.Count++;
            }
            texture = entry?.Texture;
            return result;
        }

        public void Put(CompositeKey<object, IconResolution> key, Texture texture)
        {
            _cache.Add(key, new CacheEntry(texture));
        }

        public void Return(CompositeKey<object, IconResolution> key)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                entry.Count--;
                if (entry.Count < 1)
                {
                    entry.Texture.Dispose();
                    _cache.Remove(key);
                }
            }
        }
    }
}
