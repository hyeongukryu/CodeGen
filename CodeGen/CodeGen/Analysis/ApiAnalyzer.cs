using CodeGen.Generation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;

namespace CodeGen.Analysis;

public class ApiAnalyzer(
    IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider,
    IReferenceHandlerConfiguration referenceHandlerConfiguration,
    IOptions<JsonOptions> jsonOptions)
{
    public CodeGenGenerationContext Analyze()
    {
        var context = new CodeGenGenerationContext(referenceHandlerConfiguration, jsonOptions.Value.JsonSerializerOptions);

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
