using CodeGen.Analysis;

namespace CodeGen.Web;

public class WebRequestHandler(ApiAnalyzer apiAnalyzer)
{
    private static bool GetBoolRequestParam(HttpRequest contextRequest, string name)
    {
        return contextRequest.Query.TryGetValue(name, out var values) &&
               values.Count == 1 && values[0] == "true";
    }

    private static string GetStringRequestParam(HttpRequest contextRequest, string name)
    {
        var ok = contextRequest.Query.TryGetValue(name, out var values);
        var value = values.FirstOrDefault();
        if (!ok || value == null)
        {
            throw new ArgumentException($"Missing required parameter {name}");
        }

        return value;
    }

    public Task<string> HandleApiRequest(HttpRequest contextRequest)
    {
        var context = apiAnalyzer.Analyze();
        var generateSwr = GetBoolRequestParam(contextRequest, "swr");
        var split = GetBoolRequestParam(contextRequest, "split");
        var configFilePath = GetStringRequestParam(contextRequest, "configFilePath");
        var ts = context.Compile(generateSwr, split, configFilePath);
        return Task.FromResult(ts);
    }
}