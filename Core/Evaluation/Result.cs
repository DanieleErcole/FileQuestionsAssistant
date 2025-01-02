using Core.Utils;

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
            .Select(p => {
                _correctParams.TryGetValue(p.Key, out var val);
                var equals = p.Value switch {
                    double d => (d as double?).DoubleEquals(val as double?),
                    _ => p.Value?.Equals(val) ?? false
                };
                return new KeyValuePair<string, (object?, bool)>(p.Key, (p.Value, val is null || equals));   
            }).ToDictionary()
        );

}