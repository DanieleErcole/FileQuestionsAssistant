using System.Drawing;
using Avalonia.Data.Converters;
using Core.Utils;
using Core.Utils.Converters;

namespace UI.Utils.Converters;

public static class ParamsConverters {

    public static readonly FuncValueConverter<object?, string> AnyConverter = new(x => x switch {
        Origin o => o.ToFriendlyString(),
        Alignment a => a.ToFriendlyString(),
        Color c => c.ToHexString(),
        _ => x?.ToString() ?? Lang.Lang.AnyText
    });
    
    public static readonly FuncValueConverter<object?, string> NoneConverter = new(x => x switch {
        Origin o => o.ToFriendlyString(),
        Alignment a => a.ToFriendlyString(),
        Color c => c.ToHexString(),
        _ => x?.ToString() ?? Lang.Lang.NoneText
    });
    
    public static readonly FuncValueConverter<object?, bool> NullVisibleConverter = new(x => x is not null);

}