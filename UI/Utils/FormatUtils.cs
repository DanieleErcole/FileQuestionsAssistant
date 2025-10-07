using System;

namespace UI.Utils;

public class FormatUtils {
    public static readonly Func<double, string> IntFormat = input => Math.Max(0, (int) input).ToString();
}