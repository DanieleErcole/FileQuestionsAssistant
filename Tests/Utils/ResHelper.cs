using Core.Evaluation;

namespace Tests.Utils;

public static class ResHelper {
    public static void LogResult(Result res) {
        foreach (var p in res.EachParamsWithRes()) {
            foreach (var (key, value) in p)
                Console.WriteLine($"{key}: {value}");
            Console.WriteLine("---------------");
        }
    }
}