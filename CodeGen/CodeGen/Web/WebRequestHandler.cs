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

    private static string? GetStringRequestParamOptional(HttpRequest contextRequest, string name)
    {
        var ok = contextRequest.Query.TryGetValue(name, out var values);
        var value = values.FirstOrDefault();
        if (!ok || value == null)
        {
            return null;
        }

        return value;
    }

    public Task<string> HandleApiRequest(HttpRequest contextRequest)
    {
        var context = apiAnalyzer.Analyze();

        var tag = GetStringRequestParamOptional(contextRequest, "tag");
        var format = GetStringRequestParam(contextRequest, "format");

        if (format == "typescript-api")
        {
            var generateSwr = GetBoolRequestParam(contextRequest, "swr");
            var split = GetBoolRequestParam(contextRequest, "split");
            var configFilePath = GetStringRequestParam(contextRequest, "configFilePath");
            var ts = context.Compile(generateSwr, split, configFilePath, tag);
            return Task.FromResult(ts);
        }

        if (format == "openapi-json")
        {
            return Task.FromResult("{}");
        }

        throw new ArgumentException($"Unsupported format {format}");
    }

    public Task<CodeGenConfig> HandleConfigRequest(HttpRequest contextRequest)
    {
        var context = apiAnalyzer.Analyze();
        var tags = context.GetTags();
        var config = new CodeGenConfig
        {
            Tags = tags
        };
        return Task.FromResult(config);
    }
}