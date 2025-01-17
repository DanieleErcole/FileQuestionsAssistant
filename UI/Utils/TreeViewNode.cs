using System.Collections.Generic;
using System.Linq;

namespace UI.Utils;

public class TreeViewNode(string title, object? value, bool? res, List<(string, object?, bool?)>? sub, bool isFirst = false) {
    
    public string? Title { get; } = isFirst ? value?.ToString() : title;
    public object? Value { get; } = isFirst ? null : value;
        
    public bool IsFirst { get; } = !isFirst;
    public bool? IsCorrect { get; } = res;
    public string? ResIcon { get; } = res switch {
        true => "Checkmark",
        false => "Dismiss",
        _ => null
    };
        
    public List<TreeViewNode>? SubNodes { get; } = sub?.Select(e => new TreeViewNode(e.Item1, e.Item2, e.Item3, null)).ToList();
    
}