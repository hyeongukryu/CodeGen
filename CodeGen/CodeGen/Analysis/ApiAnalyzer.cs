using CodeGen.Generation;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace CodeGen.Analysis;

public class ApiAnalyzer
{
    private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;
    private readonly IReferenceHandlerConfiguration _referenceHandlerConfiguration;

    public ApiAnalyzer(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider,
        IReferenceHandlerConfiguration referenceHandlerConfiguration)
    {
        _apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
        _referenceHandlerConfiguration = referenceHandlerConfiguration;
    }

    public TypeScriptGenerationContext Analyze()
    {
        var context = new TypeScriptGenerationContext(_referenceHandlerConfiguration);

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