using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace CodeGen.Analysis;

public class ApiAnalyzer
{
    private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;

    public ApiAnalyzer(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider)
    {
        _apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
    }
    
    
}