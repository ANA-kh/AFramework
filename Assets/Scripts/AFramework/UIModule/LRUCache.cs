using System.Collections.Generic;

namespace AFramework.UIModule
{
    public class LRUCache<TKey, TValue>
    {
        private readonly int capacity;
        private readonly Dictionary<TKey, LinkedListNode<CacheItem>> cache;
        private readonly LinkedList<CacheItem> lruList;

        public LRUCache(int capacity)
        {
            this.capacity = capacity;
            this.cache = new Dictionary<TKey, LinkedListNode<CacheItem>>(capacity);
            this.lruList = new LinkedList<CacheItem>();
        }

        public TValue Get(TKey key)
        {
            if (cache.TryGetValue(key, out var node))
            {
                var value = node.Value;
                lruList.Remove(node);
                lruList.AddFirst(node);
                return value.Value;
            }

            return default(TValue);
        }

        public void Add(TKey key, TValue value)
        {
            if (cache.Count >= capacity)
            {
                RemoveLast();
            }

            var cacheItem = new CacheItem { Key = key, Value = value };
            var node = new LinkedListNode<CacheItem>(cacheItem);
            lruList.AddFirst(node);
            cache.Add(key, node);
        }

        private void RemoveLast()
        {
            var lastNode = lruList.Last;
            cache.Remove(lastNode.Value.Key);
            lruList.RemoveLast();
        }

        private class CacheItem
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
        }
    }
}