# Lazy Loading in C#

## Overview
This repository demonstrates **lazy loading** in C# with a real-world, organic example. Lazy loading defers expensive operations until they are needed, improving performance and resource management.

## Why Lazy Loading?
- **Optimized Performance**: Prevents unnecessary data fetching.
- **Efficient Memory Usage**: Loads data on demand rather than preloading.
- **Thread Safety**: Ensures safe access in concurrent environments.

## Example Implementation
This example showcases lazy loading in a scenario where metadata retrieval is expensive and should only be performed when necessary.

### **Struct with Identity Comparison**
Defines an object with unique identifiers for efficient lookup.

```csharp
struct Phase : IEquatable<Phase>
{
    public int PhaseId { get; set; }
    public string? PhaseType { get; set; }
    public string? ProductLetter { get; set; }

    public override readonly bool Equals(object? obj) => obj is Phase other && PhaseId == other.PhaseId;
    public readonly bool Equals(Phase other) => PhaseId == other.PhaseId;
    public override readonly int GetHashCode() => PhaseId.GetHashCode();
}
```

### **Lazy Loading Metadata**
Instead of immediately fetching metadata, a `Lazy<Task<T>>` is used to **defer execution** until needed.

```csharp
static class DataProvider
{
    private static Lazy<Task<List<MetaData>>> _metaData =
        new(FetchMetaData, LazyThreadSafetyMode.ExecutionAndPublication);

    private static async Task<List<MetaData>> FetchMetaData()
    {
        try
        {
            return await MetaData.GetDataModelList();
        }
        catch
        {
            await ResetMetaData();
            throw;
        }
    }

    private static async Task ResetMetaData()
    {
        _metaData = new(FetchMetaData, LazyThreadSafetyMode.ExecutionAndPublication);
        await _metaData.Value;
    }

    public static async Task<Dictionary<Phase, T>> GetDataDictionary<T>() where T : struct =>
        (await _metaData.Value)
            .ToDictionary(data => new Phase 
                { 
                     PhaseId = data.Id, 
                     PhaseType = data.Type, 
                     ProductLetter = data.Letter 
                }, _ => default(T));
}
```

## Key Takeaways
- **Lazy Initialization** (`Lazy<Task<T>>`) ensures that data is fetched only when accessed.
- **Automatic Reset on Failure** helps recover from errors.
- **Thread Safety Mode** prevents race conditions in concurrent scenarios.
- **Efficient Dictionary Mapping** allows quick lookups while leveraging lazy-loaded data.

## Conclusion
This example illustrates how **lazy loading** can be **organically integrated** into real-world C# applications to optimize performance and resource utilization.

---
🚀 **Optimize your applications with lazy loading today!**
