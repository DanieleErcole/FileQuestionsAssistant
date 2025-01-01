using System;
using Core.Questions.Powerpoint;

namespace UI.Utils;

public static class OriginExtensions {
    public static string ToFriendlyString(this Origin origin) => origin switch {
        Origin.SlideCenter => Lang.Lang.OriginCenterText,
        Origin.TopLeftCorner => Lang.Lang.OriginTopLeftCornerText,
        _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
    };
}