using Hashtables;

namespace HashTables.HashTables;

public class HashTableWithChaining<TKey, TValue> : IHashTable<TKey, TValue>
{

    public string Name { get; } = "HashTable with chaining";
    private LinkedList<KeyValuePair<TKey, TValue>>[] _buckets;
    private int _count;

    public HashTableWithChaining(int capacity)
    {
        _buckets = new LinkedList<KeyValuePair<TKey, TValue>>[capacity];
    }

    public int Count => _count;

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out var value))
            {
                return value;
            }
            throw new KeyNotFoundException();
        }
        set
        {
            Add(key, value, overwrite: true);
        }
    }

    public void Add(TKey key, TValue value)
    {
        Add(key, value, overwrite: false);
    }

    public void Add(TKey key, TValue value, bool overwrite)
    {
        if (_count >= _buckets.Length * 0.75)
        {
            Resize();
        }

        var bucketIndex = GetBucketIndex(key);
        LinkedList<KeyValuePair<TKey, TValue>> bucket = _buckets[bucketIndex];

        if (bucket == null)
        {
            bucket = new LinkedList<KeyValuePair<TKey, TValue>>();
            _buckets[bucketIndex] = bucket;
        }
        else
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> lastNode = null;
            var currentNode = bucket.First;
            var index = 0;
            while (currentNode?.Value.Key.Equals(key) == false)
            {
                lastNode = currentNode;
                currentNode = currentNode.Next;
                index++;
            }

            if (currentNode != null)
            {
                if(!overwrite) throw new ArgumentException("An item with the same key has already been added.");
                if (lastNode == null)
                {
                    bucket.AddLast(new KeyValuePair<TKey, TValue>(key, value));
                }
                else
                {
                    bucket.Remove(currentNode);
                    bucket.AddAfter(lastNode, new KeyValuePair<TKey, TValue>(key, value));
                }
            }
            else
            {
                bucket.AddLast(new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        bucket.AddLast(new KeyValuePair<TKey, TValue>(key, value));
        _count++;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        var bucketIndex = GetBucketIndex(key);
        LinkedList<KeyValuePair<TKey, TValue>> bucket = _buckets[bucketIndex];

        if (bucket != null)
        {
            foreach (KeyValuePair<TKey, TValue> pair in bucket)
            {
                if (pair.Key.Equals(key))
                {
                    value = pair.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    public bool Delete(TKey key)
    {
        var bucketIndex = GetBucketIndex(key);
        LinkedList<KeyValuePair<TKey, TValue>> bucket = _buckets[bucketIndex];

        if (bucket != null)
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> node = bucket.First;

            while (node != null)
            {
                if (node.Value.Key.Equals(key))
                {
                    bucket.Remove(node);
                    _count--;
                    return true;
                }
                node = node.Next;
            }
        }

        return false;
    }

    public IEnumerable<TKey> GetAllKeys()
    {
        var keys = new List<TKey>();
        foreach (var bucket in _buckets)
        {
            if (bucket != null)
            {
                foreach (KeyValuePair<TKey, TValue> pair in bucket)
                {
                    keys.Add(pair.Key);
                }
            }
        }

        return keys;
    }

    private int GetBucketIndex(TKey key)
    {
        var hash = key.GetHashCode();
        return Math.Abs(hash) % _buckets.Length;
    }

    private void Resize()
    {
        var newCapacity = _buckets.Length * 2;
        var newBuckets = new LinkedList<KeyValuePair<TKey, TValue>>[newCapacity];

        foreach (LinkedList<KeyValuePair<TKey, TValue>> bucket in _buckets.Where(x => x != null))
        {
            foreach (KeyValuePair<TKey, TValue> pair in bucket)
            {
                var index = GetBucketIndex(pair.Key);
                newBuckets[index] ??= new LinkedList<KeyValuePair<TKey, TValue>>();
                newBuckets[index].AddLast(pair);
            }
        }
        _buckets = newBuckets;
    }
}