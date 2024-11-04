using Core.Questions;

namespace Core.Evaluation; 

public class Result {
    
    public bool IsSuccessful => _params.All(p => p.Value.Equals(_paramsFromFile[p.Key]));
    private readonly Dictionary<string, object> _params;
    private readonly Dictionary<string, object> _paramsFromFile;

    public Result(Dictionary<string, object> originalParams, Dictionary<string, object> paramsFromFile) {
        _params = originalParams;
        _paramsFromFile = paramsFromFile;
    }

    public (T, T) GetParamComparison<T>(string name) where T : notnull {
        return (_params.Get<T>(name), _paramsFromFile.Get<T>(name));
    }

}