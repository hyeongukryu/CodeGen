using System.Text;
using CodeGen.Analysis;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CodeGen.Generation;

public class TypeScriptGenerationContext
{
    private readonly List<CodeGenController> _controllers = new();
    private readonly List<string> _errorMessages = new();

    public void AddAction(ApiDescription apiDescription)
    {
        if (apiDescription.ActionDescriptor is not ControllerActionDescriptor api)
        {
            throw new NotImplementedException();
        }

        var controller = EnsureControllerExists(api.ControllerName);
        if (api.AttributeRouteInfo?.Template == null)
        {
            throw new NotImplementedException();
        }

        if (api.ActionConstraints is not { Count: 1 })
        {
            _errorMessages.Add("ActionConstraints " + api.ControllerName + " " + api.ActionName);
            return;
        }

        if (api.ActionConstraints.First() is not HttpMethodActionConstraint httpMethodActionConstraint)
        {
            throw new NotImplementedException();
        }

        if (httpMethodActionConstraint.HttpMethods.Count() != 1)
        {
            _errorMessages.Add("HttpMethods " + api.ControllerName + " " + api.ActionName);
            return;
        }

        var parameters = api.Parameters.Select(p =>
        {
            if (p is not ControllerParameterDescriptor descriptor)
            {
                throw new NotImplementedException();
            }

            return descriptor;
        }).ToList();

        var bodyParameters = (from p in parameters
            where p.BindingInfo?.BindingSource?.Id == "Body"
            select p).ToList();
        if (bodyParameters.Count > 1)
        {
            throw new NotImplementedException();
        }

        var pathParameters = from p in parameters
            where p.BindingInfo?.BindingSource?.Id == "Path"
            select p;

        var queryParameters = from p in parameters
            where p.BindingInfo?.BindingSource?.Id == "Query"
            select p;

        if (apiDescription.SupportedResponseTypes.Count > 1)
        {
            throw new NotImplementedException();
        }

        var responseType = apiDescription.SupportedResponseTypes.FirstOrDefault()?.Type;

        controller.Actions.Add(new CodeGenAction(controller, api.ActionName,
            api.AttributeRouteInfo.Template,
            HttpMethods.GetCanonicalizedValue(httpMethodActionConstraint.HttpMethods.First()),
            bodyParameters.FirstOrDefault(),
            pathParameters.ToList(), queryParameters.ToList(), responseType));
    }

    private CodeGenController EnsureControllerExists(string controllerName)
    {
        var controller = _controllers.Find(c => c.Name == controllerName);
        if (controller != null)
        {
            return controller;
        }

        controller = new CodeGenController(controllerName);
        _controllers.Add(controller);
        return controller;
    }

    private static void AddPrimitiveTypes(ICollection<string> converterMethodNames, ICollection<string> definitionNames)
    {
        definitionNames.Add("string");
        definitionNames.Add("number");
        definitionNames.Add("_Dayjs");
        definitionNames.Add("boolean");
        definitionNames.Add("bigint");
        converterMethodNames.Add("_convert_string_string");
        converterMethodNames.Add("_convert_string_number");
        converterMethodNames.Add("_convert_string_bigint");
        converterMethodNames.Add("_convert_boolean_boolean");
        converterMethodNames.Add("_convert_string__Dayjs");
    }

    private (ICollection<string>, ICollection<string>, ICollection<string>, ICollection<string>) Generate()
    {
        ICollection<string> converterNames = new List<string>();
        ICollection<string> converterCodes = new List<string>();
        ICollection<string> definitionNames = new List<string>();
        ICollection<string> definitionCodes = new List<string>();
        ICollection<string> controllerCodes = new List<string>();
        ICollection<string> urlBuilderNames = new List<string>();
        ICollection<string> urlBuilderCodes = new List<string>();

        AddPrimitiveTypes(converterNames, definitionNames);

        var definitionGenerator = new TypeScriptDefinitionGenerator(definitionNames, definitionCodes);
        var converterGenerator = new TypeScriptConverterGenerator(converterNames, converterCodes, definitionGenerator);

        foreach (var controller in _controllers)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"export const {controller.Name} = {{");
            foreach (var action in controller.Actions)
            {
                if (action.ResponseType == null)
                {
                    continue;
                }

                var responseType = action.ResponseType.IsEnumerable()
                    ? action.ResponseType.GetEnumerableElementType()!
                    : action.ResponseType;
                var responseTypeIsEnumerable = action.ResponseType.IsEnumerable();
                var responseTypeName =
                    TypeScriptHelper.GetFullWebAppTypeName(responseType, responseTypeIsEnumerable, false);
                converterGenerator.GenerateIfNotExists(responseType, responseTypeIsEnumerable, false);
                var converterName = TypeScriptHelper.GetConverterName(responseType, responseTypeIsEnumerable, false);
                var actionName = TypeScriptHelper.GetPropertyName(action.Name);
                var urlBuilderName = TypeScriptHelper.GetUrlName(action);

                builder.AppendLine($"    async {actionName}(): Promise<{responseTypeName}> {{");
                builder.AppendLine($"        const _url = {urlBuilderName}();");
                builder.AppendLine($"        const _response = await _http.{action.HttpMethod.ToLower()}(_url);");
                builder.AppendLine(
                    $"        return _restoreCircularReferences({converterName}(_response.data), _createObject);");
                builder.AppendLine("    },");
            }

            builder.AppendLine($"}}");
            controllerCodes.Add(builder.ToString());
        }

        return (controllerCodes, definitionCodes, converterCodes, urlBuilderCodes);
    }

    public string Compile()
    {
        var (controllerCodes, definitionCodes, converterCodes, urlBuilderCodes) = Generate();

        var builder = new StringBuilder();
        var separator = Environment.NewLine + Environment.NewLine;

        if (_errorMessages.Any())
        {
            builder.AppendLine(string.Join(separator, _errorMessages));
        }

        var assembly = typeof(TypeScriptGenerationContext).Assembly;
        var resource = assembly.GetManifestResourceStream("CodeGen.Generation.preamble.ts");
        if (resource != null)
        {
            builder.AppendLine(new StreamReader(resource, Encoding.UTF8).ReadToEnd());
        }

        builder.AppendLine();
        builder.AppendLine("// API");
        builder.AppendLine(string.Join(separator, controllerCodes));
        builder.AppendLine();
        builder.AppendLine("// Types");
        builder.AppendLine(string.Join(separator, definitionCodes));
        builder.AppendLine();
        builder.AppendLine("// Converters");
        builder.AppendLine(string.Join(separator, converterCodes));
        builder.AppendLine();
        builder.AppendLine("// URL builders");
        builder.AppendLine(string.Join(separator, urlBuilderCodes));

        return builder.ToString();
    }
}