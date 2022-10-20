using CodeGen.Analysis;

namespace CodeGen.Web;

public class WebRequestHandler
{
    private readonly ApiAnalyzer _apiAnalyzer;

    public WebRequestHandler(ApiAnalyzer apiAnalyzer)
    {
        _apiAnalyzer = apiAnalyzer;
    }

    private static bool GetRequestParam(HttpRequest contextRequest, string name)
    {
        return contextRequest.Query.TryGetValue(name, out var values) &&
               values.Count == 1 && values[0] == "true";
    }

    public Task<string> HandleApiRequest(HttpRequest contextRequest)
    {
        var context = _apiAnalyzer.Analyze();
        var generateSwr = GetRequestParam(contextRequest, "swr");
        var split = GetRequestParam(contextRequest, "split");
        var ts = context.Compile(generateSwr, split);
        return Task.FromResult(ts);
    }
}