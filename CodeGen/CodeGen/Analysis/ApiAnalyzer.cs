using CodeGen.Generation;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace CodeGen.Analysis;

public class ApiAnalyzer
{
    private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;

    public ApiAnalyzer(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider)
    {
        _apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
    }

    public TypeScriptGenerationContext Analyze()
    {
        var context = new TypeScriptGenerationContext();

        foreach (var group in _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items)
        {
            foreach (var api in group.Items)
            {
                context.AddAction(api);
            }
        }

        return context;
    }
}