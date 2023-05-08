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

        private readonly Dictionary<object, CacheEntry> _cache = new();

        public bool TryGetTexture(object @object, out Texture? texture)
        {
            bool result = _cache.TryGetValue(@object, out var entry);
            if (entry != null)
            {
                entry.Count++;
            }
            texture = entry?.Texture;
            return result;
        }

        public void Put(object @object, Texture texture)
        {
            _cache.Add(@object, new CacheEntry(texture));
        }

        public void Return(object @object)
        {
            if (_cache.TryGetValue(@object, out var entry))
            {
                entry.Count--;
                if (entry.Count < 1)
                {
                    entry.Texture.Dispose();
                    _cache.Remove(@object);
                }
            }
        }
    }
}
