using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace BloomFilterSample;

internal sealed class BloomFilter
{
    private readonly BitArray _bitArray;
    private readonly Func<string, int>[] _hashFunctions;

    public BloomFilter(int size, int hashFunctionCount)
    {
        _bitArray = new BitArray(size);
        _hashFunctions = CreateHashFunctions(hashFunctionCount);
    }
    
    public void Add(string item)
    {
        foreach (var hashFunction in _hashFunctions)
        {
            var index = hashFunction(item);
            _bitArray[index] = true;
        }
    }
    
    public bool MightContain(string item)
    {
        return _hashFunctions.Select(hashFunction => hashFunction(item))
            .All(index => _bitArray[index]);
    }

    private Func<string,int>[] CreateHashFunctions(int hashFunctionCount)
    {
        var hashFunctions = new Func<string, int>[hashFunctionCount];
        for (int i = 0; i < hashFunctionCount; i++)
        {
            int index = i;  // Capture the current index for the closure
            hashFunctions[i] = (item) =>
            {
                var md5Hash = MD5.HashData(Encoding.UTF8.GetBytes(item));
                var sha1Hash = SHA1.HashData(Encoding.UTF8.GetBytes(item));

                var combinedHash = BitConverter.ToInt32(md5Hash, index % 4) ^ BitConverter.ToInt32(sha1Hash, index % 4);
                return Math.Abs(combinedHash % _bitArray.Length);
            };
        }

        return hashFunctions;
    }
}