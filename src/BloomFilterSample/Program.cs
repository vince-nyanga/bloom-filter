using BloomFilterSample;

var bloomFilter = new BloomFilter(100_000, 5);
bloomFilter.Add("apple");

Console.WriteLine(bloomFilter.MightContain("apple")); // True
Console.WriteLine(bloomFilter.MightContain("banana")); // False