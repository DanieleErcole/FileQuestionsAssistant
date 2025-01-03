namespace Core.Utils;

public static class ParamsExtensions {
    public static T? Get<T>(this Dictionary<string, object?> d, string index) {
        if (d[index] is { } el && !el.GetType().IsAssignableTo(typeof(T)))
            throw new InvalidCastException($"Cannot cast type {el.GetType()} to {typeof(T)}");

        if (d[index] is null) 
            return default;
        
        return (T?) d[index];
    }
}