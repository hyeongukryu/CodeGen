namespace CodeGen.Web;

public class WebRequestHandler
{
    public Task<string> HandleWebAppRequest()
    {
        return Task.FromResult("code-gen");
    }
    
    public Task<string> HandleApiRequest()
    {
        return Task.FromResult("api");
    }
}