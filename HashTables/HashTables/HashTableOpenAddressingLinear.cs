using Hashtables;

namespace HashTables.HashTables;

public class HashTableOpenAddressingLinear<TKey, TValue> : IHashTable<TKey, TValue>
{
    public string Name { get; } = "HashTable with open addressing using linear probing";
    
    private int _capacity;
    private int _size;
    private KeyValuePair<TKey, TValue>[] _items;

    public HashTableOpenAddressingLinear(int capacity)
    {
        _capacity = capacity;
        _size = 0;
        _items = new KeyValuePair<TKey, TValue>[_capacity];
    }

    public void Add(TKey key, TValue value)
    {
        var index = GetIndex(key);

        while (_items[index].Key != null)
        {
            if (_items[index].Key.Equals(key))
            {
                throw new ArgumentException("An item with the same key has already been added.");
            }

            index = (index + 1) % _capacity;
        }

        _items[index] = new KeyValuePair<TKey, TValue>(key, value);
        _size++;

        if (_size >= _capacity * 0.75)
        {
            Resize(_capacity * 2);
        }
    }

    public bool ContainsKey(TKey key)
    {
        var index = GetIndex(key);

        while (_items[index].Key != null)
        {
            if (_items[index].Key.Equals(key))
            {
                return true;
            }

            index = (index + 1) % _capacity;
        }

        return false;
    }


    public bool TryGetValue(TKey key, out TValue value)
    {
        var index = GetIndex(key);

        while (_items[index].Key != null)
        {
            if (_items[index].Key.Equals(key))
            {
                value = _items[index].Value;
                return true;
            }

            index = (index + 1) % _capacity;
        }

        value = default(TValue);
        return false;
    }

    public bool Delete(TKey key)
    {
        var index = GetIndex(key);

        while (_items[index].Key != null)
        {
            if (_items[index].Key.Equals(key))
            {
                _items[index] = new KeyValuePair<TKey, TValue>();
                _size--;
                return true;
            }

            index = (index + 1) % _capacity;
        }

        return false;
    }

    public IEnumerable<TKey> GetAllKeys()
    {
        for (var i = 0; i < _capacity; i++)
        {
            if (_items[i].Key != null)
            {
                yield return _items[i].Key;
            }
        }
    }

    private int GetIndex(TKey key)
    {
        var hashCode = key.GetHashCode();
        var index = hashCode % _capacity;

        if (index < 0)
        {
            index += _capacity;
        }

        return index;
    }

    private void Resize(int newCapacity)
    {
        var oldItems = _items;
        _items = new KeyValuePair<TKey, TValue>[newCapacity];
        _capacity = newCapacity;
        _size = 0;

        foreach (KeyValuePair<TKey, TValue> item in oldItems)
        {
            if (item.Key != null)
            {
                Add(item.Key, item.Value);
            }
        }
    }
}