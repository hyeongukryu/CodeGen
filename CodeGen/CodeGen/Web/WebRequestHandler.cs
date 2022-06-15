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
        var ts = context.Compile();
        return Task.FromResult(ts);
    }
}