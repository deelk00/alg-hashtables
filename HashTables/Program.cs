// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using Hashtables;
using HashTables.HashTables;

var testSizes = new List<int>()
{
    1000,
    100000,
    10000000
};

var hashTableTypes = new List<Type>()
{
    typeof(HashTableWithChaining<string, int>),
    typeof(HashTableOpenAddressingLinear<string, int>),
    typeof(HashTableOpenAddressingQuadratic<string, int>),
    typeof(HashTableOpenAddressingDoubleHashing<string, int>),
    typeof(HashTableCuckoo<string, int>)
};

var resultSb = new StringBuilder();
const string resultPrintPath = @".\results.txt";

foreach (var hashTableType in hashTableTypes)
{
    foreach (var testSize in testSizes)
    {
        // create new hashtable through reflection with testSize

        if (Activator.CreateInstance(hashTableType, new object [] {testSize}) is not IHashTable<string, int> hashTable) 
            throw new TypeLoadException("the correct type could not be loaded. Make sure every HashTable type has a string key and int value");

        var insertTime = TestHashTableInsert(hashTable, testSize);
        var getTime = TestHashTableGet(hashTable);
        var deleteTime = TestHashTableDelete(hashTable);
        
        Console.WriteLine($"------------{hashTable.Name}------------");
        Console.WriteLine($"Test results for a capacity of {testSize} in ms:");
        Console.WriteLine($"Insertion: {insertTime.TotalMilliseconds}");
        Console.WriteLine($"Searching: {getTime.TotalMilliseconds}");
        Console.WriteLine($"Deletion: {deleteTime.TotalMilliseconds}");
        Console.WriteLine("------------------------------------");
        Console.WriteLine();

        resultSb.AppendLine(hashTable.Name);
        resultSb.AppendLine("test size: " + testSize);
        resultSb.AppendLine("Insert: " + insertTime.TotalMilliseconds);
        resultSb.AppendLine("Get: " + getTime.TotalMilliseconds);
        resultSb.AppendLine("Delete: " + deleteTime.TotalMilliseconds);
        resultSb.AppendLine("---------------------");
        resultSb.AppendLine();
    }
}
File.WriteAllText(resultPrintPath, resultSb.ToString());

TimeSpan TestHashTableInsert(IHashTable<string, int> hashTable, int testSize)
{
    var rand = new Random();
    var pairs = new List<KeyValuePair<string, int>>();
    for (var i = 0; i < testSize; i++)
    {
        var key = Guid.NewGuid().ToString();
        var value = rand.Next(10000);
        pairs.Add(new KeyValuePair<string, int>(key, value));
    }

    // Insert the key-value pairs into the hashtable and measure the elapsed time
    var sw = new Stopwatch();
    sw.Start();
    foreach (var pair in pairs)
    {
        hashTable.Add(pair.Key, pair.Value);
    }
    sw.Stop();

    // Output the elapsed time to the console
    return sw.Elapsed;
}

TimeSpan TestHashTableGet(IHashTable<string, int> hashTable)
{
    var keys = hashTable.GetAllKeys();

    var sw = new Stopwatch();
    sw.Start();
    foreach (var key in keys)
    {
        hashTable.TryGetValue(key, out _);
    }
    sw.Stop();

    return sw.Elapsed;
}

TimeSpan TestHashTableDelete(IHashTable<string, int> hashTable)
{
    var keys = hashTable.GetAllKeys();

    var sw = new Stopwatch();
    sw.Start();
    foreach (var key in keys)
    {
        hashTable.Delete(key);
    }
    sw.Stop();

    return sw.Elapsed;
}