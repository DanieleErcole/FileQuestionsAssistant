namespace Core.Evaluation; 

public class Result {
    
    public bool IsSuccessful { get; }
    private readonly Dictionary<string, object?> _correctParams;
    private readonly List<Dictionary<string, object?>> _stylesFromFile;

    public Result(Dictionary<string, object?> originalParams, List<Dictionary<string, object?>> stylesFromFile, bool isSuccessful) {
        IsSuccessful = isSuccessful;
        _correctParams = originalParams;
        _stylesFromFile = stylesFromFile;
    }

    public IEnumerable<(Dictionary<string, object?>, Dictionary<string, bool>)> EachStyleWithRes() => _stylesFromFile
        .Select(styleParams => (
            styleParams, 
            styleParams
                .Select(p => new KeyValuePair<string, bool>(p.Key, p.Value == _correctParams[p.Key]))
                .ToDictionary()
            ));

}