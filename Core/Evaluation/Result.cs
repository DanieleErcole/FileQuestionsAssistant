namespace Core.Evaluation; 

public class Result {
    
    public bool IsSuccessful { get; }
    private readonly Dictionary<string, object?> _correctParams;
    private readonly List<Dictionary<string, object?>> _paramsFromFile;

    public Result(Dictionary<string, object?> originalParams, List<Dictionary<string, object?>> paramsFromFile, bool isSuccessful) {
        IsSuccessful = isSuccessful;
        _correctParams = originalParams;
        _paramsFromFile = paramsFromFile;
    }

    public IEnumerable<Dictionary<string, (object?, bool)>> EachParamsWithRes() => _paramsFromFile
        .Select(styleParams => styleParams
            .Select(p => new KeyValuePair<string, (object?, bool)>(p.Key, (p.Value, _correctParams[p.Key] is null || p.Value == _correctParams[p.Key])))
            .ToDictionary()
        );

}