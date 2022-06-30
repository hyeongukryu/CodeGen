using CodeGen.Analysis;

namespace CodeGen.Web;

public class WebRequestHandler
{
    private readonly ApiAnalyzer _apiAnalyzer;

    public WebRequestHandler(ApiAnalyzer apiAnalyzer)
    {
        _apiAnalyzer = apiAnalyzer;
    }

    public Task<string> HandleApiRequest(HttpRequest contextRequest)
    {
        var context = _apiAnalyzer.Analyze();
        var generateSwr = contextRequest.Query.TryGetValue("swr", out var values) &&
                          values.Count == 1 && values[0] == "true";
        var ts = context.Compile(generateSwr);
        return Task.FromResult(ts);
    }
}