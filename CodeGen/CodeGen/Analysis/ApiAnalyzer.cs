using CodeGen.Generation;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace CodeGen.Analysis;

public class ApiAnalyzer(
    IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider,
    IReferenceHandlerConfiguration referenceHandlerConfiguration)
{
    public TypeScriptGenerationContext Analyze()
    {
        var context = new TypeScriptGenerationContext(referenceHandlerConfiguration);

        foreach (var group in apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items)
        {
            foreach (var api in group.Items)
            {
                context.AddAction(api);
            }
        }

        return context;
    }
}