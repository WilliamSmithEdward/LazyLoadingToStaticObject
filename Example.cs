class DMPhaseMetaData
{
    public static async Task<List<DMPhaseMetaData>> GetDataModelList()
    {
        return await Program.AndromedaSSI.ExecuteQueryToObjectListAsync<DMPhaseMetaData>($"""
            SELECT
                PhaseN AS PhaseId,
                PhaseType,
                Product AS ProductLetter
            FROM [Production].[reference].[PhaseNumberMeta]
        """);
    }

    public int PhaseId { get; set; }
    public string? PhaseType { get; set; }
    public string? ProductLetter { get; set; }
}

struct Phase : IEquatable<Phase>
{
    public int PhaseId { get; set; }
    public string? PhaseType { get; set; }
    public string? ProductLetter { get; set; }

    public override readonly bool Equals(object? obj) => obj is Phase other && PhaseId == other.PhaseId;
    public readonly bool Equals(Phase other) => PhaseId == other.PhaseId;
    public override readonly int GetHashCode() => PhaseId.GetHashCode();
}

static class PhaseDataProvider
{
    private static Lazy<Task<List<DMPhaseMetaData>>> _phaseMetaData =
        new(FetchPhaseMetaData, LazyThreadSafetyMode.ExecutionAndPublication);

    private static async Task<List<DMPhaseMetaData>> FetchPhaseMetaData()
    {
        try
        {
            return await DMPhaseMetaData.GetDataModelList();
        }

        catch
        {
            await ResetPhaseMetaData();
            throw;
        }
    }

    private static async Task ResetPhaseMetaData()
    {
        _phaseMetaData = new(FetchPhaseMetaData, LazyThreadSafetyMode.ExecutionAndPublication);
        await _phaseMetaData.Value;
    }

    public static async Task<Dictionary<Phase, T>> GetPhaseDictionary<T>() where T : struct =>
        (await _phaseMetaData.Value)
            .ToDictionary(phase => 
                new Phase 
                { 
                     PhaseId = phase.PhaseId
                    ,PhaseType = phase.PhaseType
                    ,ProductLetter = phase.ProductLetter
                }
            , _ => default(T));
}
