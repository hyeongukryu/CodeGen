using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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

    private (ICollection<string>, ICollection<string>, ICollection<string>, ICollection<string>)
        Generate(bool generateSwr)
    {
        ICollection<string> converterNames = new List<string>();
        ICollection<string> converterCodes = new List<string>();
        ICollection<string> definitionNames = new List<string>();
        ICollection<string> definitionCodes = new List<string>();
        ICollection<string> controllerCodes = new List<string>();
        ICollection<string> urlBuilderCodes = new List<string>();
        ISet<Tuple<string, string>> definitionFullNames = new HashSet<Tuple<string, string>>();

        AddPrimitiveTypes(converterNames, definitionNames);

        var definitionGenerator =
            new TypeScriptDefinitionGenerator(definitionNames, definitionCodes, definitionFullNames, _errorMessages);
        var converterGenerator = new TypeScriptConverterGenerator(converterNames, converterCodes, definitionGenerator);

        foreach (var controller in _controllers)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"export const {controller.Name} = {{");
            foreach (var action in controller.Actions)
            {
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

                var pathTypes =
                    (from p in action.PathParameters select p.ParameterInfo.ToCodeGenType()).ToList();
                var queryTypes =
                    (from p in action.QueryParameters select p.ParameterInfo.ToCodeGenType()).ToList();
                pathTypes.ForEach(t => converterGenerator.GenerateIfNotExists(t));
                queryTypes.ForEach(t => converterGenerator.GenerateIfNotExists(t));

                var pathParameters = pathTypes.Select((t, i) =>
                    action.PathParameters.ElementAt(i).Name + ": " + t.GetFullWebAppTypeName()).ToList();
                var queryParameters = queryTypes.Select((t, i) =>
                    action.QueryParameters.ElementAt(i).Name + ": " + t.GetFullWebAppTypeName()).ToList();

                CodeGenType? payloadType = null;
                if (action.BodyParameter != null)
                {
                    payloadType = action.BodyParameter.ParameterInfo.ToCodeGenType();
                    converterGenerator.GenerateIfNotExists(payloadType);
                }

                var payloadParameter = action.BodyParameter != null && payloadType != null
                    ? $"{action.BodyParameter.Name}: {payloadType.GetFullWebAppTypeName()}"
                    : null;

                var actionParameters = pathParameters.Concat(queryParameters)
                    .Concat(payloadParameter != null ? new[] { payloadParameter } : Array.Empty<string>()).ToList();

                var pathArgs = action.PathParameters.Select(p => p.Name).ToList();
                var queryArgs = action.QueryParameters.Select(p => p.Name).ToList();
                var urlBuilderArgs = string.Join(", ", pathArgs.Concat(queryArgs));

                var actionName = action.Name.ToCamelCase();
                var urlBuilderName = action.GetUrlName();
                var responseTypeName = responseType == null ? "void" : responseType.GetFullWebAppTypeName();
                builder.AppendLine(
                    $"    async {actionName}({string.Join(", ", actionParameters)}): Promise<{responseTypeName}> {{");

                var payloadArgument = "";
                if (payloadType != null && action.BodyParameter != null)
                {
                    payloadArgument = ", " + payloadType.GetConverterName(true) + $"({action.BodyParameter.Name})";
                }

                builder.AppendLine($"        const _response = await _createHttp().{action.HttpMethod.ToLower()}" +
                                   $"({urlBuilderName}({urlBuilderArgs}){payloadArgument});");

                if (responseType != null)
                {
                    builder.AppendLine(
                        $"        return _restoreCircularReferences({responseType.GetConverterName(false)}(_response.data), _createObject);");
                }

                builder.AppendLine("    },");

                if (generateSwr && action.HttpMethod == "GET" && responseType != null)
                {
                    var swrParameters = actionParameters.Concat(new[]
                        { "_config: SWRConfiguration = {}", "_shouldFetch: boolean = true" });
                    builder.AppendLine($"    useSWR{actionName.ToPascalCase()}({string.Join(", ", swrParameters)}) {{");
                    builder.AppendLine($"        return _useSWR<{responseType.GetFullWebAppTypeName()}>" +
                                       $"(_shouldFetch ? {urlBuilderName}({urlBuilderArgs}) : null, " +
                                       $"{{ ..._config, use: [_createSWRMiddleware({responseType.GetConverterName(false)})] }});");
                    builder.AppendLine("    },");
                }

                var urlBuilder = new StringBuilder();
                var urlBuilderParameters = string.Join(", ", pathParameters.Concat(queryParameters));
                urlBuilder.AppendLine($"function {urlBuilderName}({urlBuilderParameters}): string {{");

                if (queryTypes.Count > 0)
                {
                    urlBuilder.AppendLine("    const _params = new URLSearchParams();");
                    for (var q = 0; q < queryTypes.Count; q++)
                    {
                        var name = queryArgs[q];
                        var queryConverter = queryTypes[q].GetConverterName(true);
                        converterGenerator.GenerateIfNotExists(queryTypes[q]);
                        var nameTemp = "_converted_" + name;
                        urlBuilder.AppendLine($"    const {nameTemp} = {queryConverter}({name});");
                        urlBuilder.AppendLine($"    if ({nameTemp} !== null) {{");
                        urlBuilder.AppendLine($"        _params.append('{name}', {nameTemp}.toString());");
                        urlBuilder.AppendLine("    }");
                    }

                    urlBuilder.AppendLine("    const _queryString = _params.toString();");
                }

                var routeTemplate = Regex.Replace(action.Template, ":(.*?)}", "}")
                    .Replace("{", "${").Replace("}", ".toString()}");
                urlBuilder.AppendLine(
                    $"    return `{routeTemplate}`" +
                    (queryArgs.Count > 0 ? "+ (_queryString.length ? '?' + _queryString : '')" : "") + ";");
                urlBuilder.Append('}');
                urlBuilderCodes.Add(urlBuilder.ToString());
            }

            builder.Append('}');
            controllerCodes.Add(builder.ToString());
        }

        return (controllerCodes, definitionCodes, converterCodes, urlBuilderCodes);
    }

    public string Compile(bool generateSwr)
    {
        var (controllerCodes, definitionCodes, converterCodes, urlBuilderCodes) = Generate(generateSwr);

        var builder = new StringBuilder();
        var separator = Environment.NewLine + Environment.NewLine;

        if (_errorMessages.Any())
        {
            builder.AppendLine("ERROR");
            builder.AppendLine();
            builder.AppendLine(string.Join(Environment.NewLine, _errorMessages));
        }

        var assembly = typeof(TypeScriptGenerationContext).Assembly;
        var resource = assembly.GetManifestResourceStream(
            generateSwr
                ? "CodeGen.Generation.preamble-swr.ts"
                : "CodeGen.Generation.preamble.ts");
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