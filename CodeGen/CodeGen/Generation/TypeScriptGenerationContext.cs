using System.Reflection;
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
            _errorMessages.Add("ActionDescriptor " + apiDescription.ActionDescriptor.Id);
            return;
        }

        if (api.MethodInfo.GetCustomAttribute<CodeGenIgnoreAttribute>() != null)
        {
            return;
        }

        var controller = EnsureControllerExists(api.ControllerName);
        if (api.AttributeRouteInfo?.Template == null)
        {
            _errorMessages.Add("Template " + api.ControllerName + " " + api.ActionName);
            return;
        }

        if (api.ActionConstraints is not { Count: 1 })
        {
            _errorMessages.Add("ActionConstraints " + api.ControllerName + " " + api.ActionName);
            return;
        }

        if (api.ActionConstraints.First() is not HttpMethodActionConstraint httpMethodActionConstraint)
        {
            _errorMessages.Add("HttpMethodActionConstraint " + api.ControllerName + " " + api.ActionName);
            return;
        }

        if (httpMethodActionConstraint.HttpMethods.Count() != 1)
        {
            _errorMessages.Add("HttpMethods " + api.ControllerName + " " + api.ActionName);
            return;
        }

        if (api.Parameters.Any(p => p is not ControllerParameterDescriptor))
        {
            _errorMessages.Add("ControllerParameterDescriptor " + api.ControllerName + " " + api.ActionName);
            return;
        }

        var parameters = api.Parameters.Select(d => (ControllerParameterDescriptor)d).ToList();

        var bodyParameters = (from p in parameters
            where p.BindingInfo?.BindingSource?.Id == "Body"
            select p).ToList();
        if (bodyParameters.Count > 1)
        {
            _errorMessages.Add("bodyParameters " + api.ControllerName + " " + api.ActionName);
            return;
        }

        var pathParameters = from p in parameters
            where p.BindingInfo?.BindingSource?.Id == "Path"
            select p;

        var queryParameters = from p in parameters
            where p.BindingInfo?.BindingSource?.Id == "Query"
            select p;

        if (apiDescription.SupportedResponseTypes.Count > 1)
        {
            _errorMessages.Add("SupportedResponseTypes " + api.ControllerName + " " + api.ActionName);
            return;
        }

        var responseType = apiDescription.SupportedResponseTypes.FirstOrDefault()?.Type;
        if (responseType == null)
        {
            if (api.MethodInfo.GetCustomAttribute(typeof(CommandAttribute)) == null)
            {
                _errorMessages.Add("ResponseType " + api.ControllerName + " " + api.ActionName);
                return;
            }
        }

        controller.Actions.Add(new CodeGenAction(controller, api.ActionName,
            api.AttributeRouteInfo.Template,
            HttpMethods.GetCanonicalizedValue(httpMethodActionConstraint.HttpMethods.First()),
            bodyParameters.FirstOrDefault(),
            pathParameters.ToList(), queryParameters.ToList(), responseType, responseType == null));
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
        converterMethodNames.Add("_convert_string_TO_string");
        converterMethodNames.Add("_convert_string_TO_number");
        converterMethodNames.Add("_convert_number_TO_string");
        converterMethodNames.Add("_convert_string_TO_bigint");
        converterMethodNames.Add("_convert_bigint_TO_string");
        converterMethodNames.Add("_convert_boolean_TO_boolean");
        converterMethodNames.Add("_convert_string_TO__Dayjs");
        converterMethodNames.Add("_convert__Dayjs_TO_string");
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
                // action.PathParameters
                // action.QueryParameters

                CodeGenType? responseType = null;
                if (action.ResponseType != null)
                {
                    if (!action.ResponseType.IsEnumerable() && action.ResponseType.GetNullableElementType() != null)
                    {
                        _errorMessages.Add("ResponseType.GetNullableElementType " + action.Controller.Name + " " +
                                           action.Name);
                        continue;
                    }

                    responseType = action.ResponseType.ToCodeGenType();
                    converterGenerator.GenerateIfNotExists(responseType);
                }

                CodeGenType? payloadType = null;
                if (action.BodyParameter != null)
                {
                    payloadType = action.BodyParameter.ParameterInfo.ToCodeGenType();
                    converterGenerator.GenerateIfNotExists(payloadType);
                }

                var payloadParameter = action.BodyParameter != null && payloadType != null
                    ? $"{action.BodyParameter.Name}: {payloadType.GetFullWebAppTypeName()}"
                    : null;
                var payloadArgument = action.BodyParameter != null && payloadType != null
                    ? $", {payloadType.GetConverterName(true)}({action.BodyParameter.Name})"
                    : "";

                var actionParameters = new List<string>();
                if (payloadParameter != null)
                {
                    actionParameters.Add(payloadParameter);
                }

                var actionName = action.Name.ToCamelCase();
                var urlBuilderName = action.GetUrlName();
                var responseTypeName = responseType == null ? "void" : responseType.GetFullWebAppTypeName();
                builder.AppendLine(
                    $"    async {actionName}({string.Join(", ", actionParameters)}): Promise<{responseTypeName}> {{");
                builder.AppendLine($"        const _url = {urlBuilderName}();");
                builder.AppendLine(
                    $"        const _response = await _http.{action.HttpMethod.ToLower()}(_url{payloadArgument});");

                if (responseType != null)
                {
                    var responseConverterName = responseType.GetConverterName(false);
                    builder.AppendLine(
                        $"        return _restoreCircularReferences({responseConverterName}(_response.data), _createObject);");
                }

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
            builder.AppendLine("ERROR");
            builder.AppendLine();
            builder.AppendLine(string.Join(Environment.NewLine, _errorMessages));
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