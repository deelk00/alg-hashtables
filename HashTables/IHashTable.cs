namespace Hashtables;

public interface IHashTable<TKey, TValue>
{
    public string Name { get; }
    public bool TryGetValue(TKey key, out TValue value);
    public void Add(TKey key, TValue value);
    public bool Delete(TKey key);
    public IEnumerable<TKey> GetAllKeys();
}