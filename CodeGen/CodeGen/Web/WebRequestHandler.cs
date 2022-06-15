namespace CodeGen.Web;

public class WebRequestHandler
{
    public Task<string> HandleApiRequest(HttpRequest contextRequest)
    {
        return Task.FromResult("api");
    }
}