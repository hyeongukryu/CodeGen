using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CodeGen.Analysis;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CodeGen.Generation;

public class CodeGenGenerationContext(
    IReferenceHandlerConfiguration referenceHandlerConfiguration,
    JsonSerializerOptions jsonSerializerOptions)
{
    private readonly List<CodeGenController> _controllers = [];
    private readonly List<string> _errorMessages = [];
    private bool _controllerNamesResolved;

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

        var controller = EnsureControllerExists(api.ControllerTypeInfo.AsType(), api.ControllerName);
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

        var tagsAttribute = api.MethodInfo.GetCustomAttribute(typeof(TagsAttribute)) as TagsAttribute;
        var tags = tagsAttribute?.Tags ?? [];

        controller.Actions.Add(new CodeGenAction(controller, api.ActionName,
            api.AttributeRouteInfo.Template,
            HttpMethods.GetCanonicalizedValue(httpMethodActionConstraint.HttpMethods.First()),
            bodyParameters.FirstOrDefault(),
            pathParameters.ToList(), queryParameters.ToList(), responseType,
            responseType == null, tags
        ));
    }

    public IEnumerable<string> GetTags()
    {
        var tags = from controller in _controllers
            from action in controller.Actions
            from tag in action.Tags
            select tag;

        return tags.Distinct().OrderBy(t => t);
    }

    private CodeGenController EnsureControllerExists(Type controllerType, string controllerName)
    {
        var controller = _controllers.Find(c => c.ControllerType == controllerType);
        if (controller != null)
        {
            return controller;
        }

        controller = new CodeGenController(controllerType, controllerName);
        _controllers.Add(controller);
        _controllerNamesResolved = false;
        return controller;
    }

    private void EnsureControllerNamesResolved()
    {
        if (_controllerNamesResolved)
        {
            return;
        }

        new ControllerNameResolver().ResolveNames(_controllers);
        _controllerNamesResolved = true;
    }

    private static IEnumerable<string> PrimitiveTypes =>
        new[] { "string", "number", "_Dayjs", "boolean", "bigint", "Uint8Array" };

    private static void AddPrimitiveTypes(ICollection<string> converterMethodNames, ICollection<string> definitionNames)
    {
        foreach (var primitiveType in PrimitiveTypes)
        {
            definitionNames.Add(primitiveType);
        }

        converterMethodNames.Add("_convert_string_TO_string");
        converterMethodNames.Add("_convert_string_TO_number");
        converterMethodNames.Add("_convert_number_TO_string");
        converterMethodNames.Add("_convert_string_TO_bigint");
        converterMethodNames.Add("_convert_bigint_TO_string");
        converterMethodNames.Add("_convert_boolean_TO_boolean");
        converterMethodNames.Add("_convert_Uint8Array_TO_string");
        converterMethodNames.Add("_convert_string_TO_Uint8Array");
        converterMethodNames.Add("_convert_string_TO__Dayjs");
        converterMethodNames.Add("_convert__Dayjs_TO_string");
    }

    private CodeGenTypeScriptApiResult GenerateTypeScriptApi(bool generateSwr, bool split, string? tag)
    {
        EnsureControllerNamesResolved();

        ICollection<string> converterNames = new List<string>();
        ICollection<string> converterCodes = new List<string>();
        ICollection<string> definitionNames = new List<string>();
        ICollection<string> definitionCodes = new List<string>();
        ICollection<string> urlBuilderCodes = new List<string>();
        ICollection<string> urlBuilderNames = new List<string>();
        ICollection<CodeGenControllerResult> controllerResults = new List<CodeGenControllerResult>();
        var typeNameResolver = new TypeScriptNameResolver(GenerationTypeCollector.CollectTypes(_controllers, tag));
        var controllerFileNameResolver = new ControllerFileNameResolver();
        controllerFileNameResolver.ResolveNames(_controllers);

        AddPrimitiveTypes(converterNames, definitionNames);

        var definitionGenerator =
            new TypeScriptDefinitionGenerator(definitionNames, definitionCodes, _errorMessages, typeNameResolver);
        var converterGenerator = new TypeScriptConverterGenerator(
            converterNames, converterCodes, definitionGenerator, referenceHandlerConfiguration, typeNameResolver);

        foreach (var controller in _controllers)
        {
            if (tag != null && !controller.Actions.Any(a => a.Tags.Contains(tag)))
            {
                // 태그 필터
                continue;
            }

            var builder = new StringBuilder();
            if (!split)
            {
                builder.AppendLine($"export const {controller.GeneratedName} = {{");
            }

            foreach (var action in controller.Actions)
            {
                if (tag != null && !action.Tags.Contains(tag))
                {
                    // 태그 필터
                    continue;
                }

                CodeGenType? responseType = null;
                if (action.ResponseType != null)
                {
                    if (!action.ResponseType.IsEnumerable() && action.ResponseType.GetNullableElementType() != null)
                    {
                        _errorMessages.Add("ResponseType.GetNullableElementType " + action.Controller.ControllerName + " " +
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
                    action.PathParameters.ElementAt(i).Name + ": " + typeNameResolver.GetFullWebAppTypeName(t)).ToList();
                var queryParameters = queryTypes.Select((t, i) =>
                    action.QueryParameters.ElementAt(i).Name + ": " + typeNameResolver.GetFullWebAppTypeName(t)).ToList();

                CodeGenType? payloadType = null;
                if (action.BodyParameter != null)
                {
                    payloadType = action.BodyParameter.ParameterInfo.ToCodeGenType();
                    converterGenerator.GenerateIfNotExists(payloadType);
                }

                var payloadParameter = action.BodyParameter != null && payloadType != null
                    ? $"{action.BodyParameter.Name}: {typeNameResolver.GetFullWebAppTypeName(payloadType)}"
                    : null;

                var actionParameters = pathParameters.Concat(queryParameters)
                    .Concat(payloadParameter != null ? new[] { payloadParameter } : Array.Empty<string>()).ToList();

                var pathArgs = action.PathParameters.Select(p => p.Name).ToList();
                var queryArgs = action.QueryParameters.Select(p => p.Name).ToList();
                var urlBuilderArgs = string.Join(", ", pathArgs.Concat(queryArgs));

                var actionName = action.Name.ToCamelCase();
                if (actionName == "delete")
                {
                    // JavaScript reserved word
                    actionName = "/* delete */ Delete";
                }

                var urlBuilderName = action.GetUrlName();
                var responseTypeName = responseType == null ? "void" : typeNameResolver.GetFullWebAppTypeName(responseType);
                var parametersWithAxiosRequestConfig = actionParameters
                    .Concat(["_axiosRequestConfig?: _AxiosRequestConfig"]);
                builder.AppendLine(
                    (split ? "export async function" : "async") +
                    $" {actionName}({string.Join(", ", parametersWithAxiosRequestConfig)}): Promise<{responseTypeName}> {{");

                var payloadArgument = "";
                if (payloadType != null && action.BodyParameter != null)
                {
                    payloadArgument = ", " + typeNameResolver.GetConverterName(payloadType, true) +
                                      $"({action.BodyParameter.Name})";
                }

                // TS6133: '_response' is declared but its value is never read.
                var declareResponseLocalVar = responseType != null ? "const _response: any = " : "";
                builder.AppendLine($"    {declareResponseLocalVar}await _createHttp().{action.HttpMethod.ToLower()}" +
                                   $"({urlBuilderName}({urlBuilderArgs}){payloadArgument}, _axiosRequestConfig);");

                if (responseType != null)
                {
                    builder.AppendLine(
                        $"    return _restoreCircularReferences({typeNameResolver.GetConverterName(responseType, false)}(_response.data), _createObject);");
                }

                builder.AppendLine("}" + (split ? "" : ","));

                if (generateSwr && action.HttpMethod == "GET" && responseType != null)
                {
                    var swrParameters = actionParameters.Concat(new[]
                        { "_config: _SWRConfiguration = {}", "_shouldFetch: boolean = true" });
                    builder.AppendLine((split ? "export function " : "") +
                                       $"useSWR{actionName.ToPascalCase()}({string.Join(", ", swrParameters)}) {{");
                    builder.AppendLine($"    return _useSWR<{typeNameResolver.GetFullWebAppTypeName(responseType)}>" +
                                       $"(_shouldFetch ? {urlBuilderName}({urlBuilderArgs}) : null, " +
                                       $"{{ ..._config, use: [_createSWRMiddleware({typeNameResolver.GetConverterName(responseType, false)})] }});");
                    builder.AppendLine("}" + (split ? "" : ","));
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
                        var queryConverter = typeNameResolver.GetConverterName(queryTypes[q], true);
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
                urlBuilderNames.Add(urlBuilderName);
            }

            if (!split)
            {
                builder.Append('}');
            }

            controllerResults.Add(new CodeGenControllerResult(
                controller.GeneratedName,
                controllerFileNameResolver.GetFileName(controller),
                builder.ToString()));
        }

        var dependencyErrors = converterGenerator.CheckDependencyErrors();
        if (dependencyErrors != null)
        {
            _errorMessages.Add(dependencyErrors);
        }

        return new CodeGenTypeScriptApiResult(controllerResults, definitionCodes, definitionNames,
            converterCodes, converterNames, urlBuilderCodes, urlBuilderNames);
    }

    private static string GetResourceString(string name)
    {
        var assembly = typeof(CodeGenGenerationContext).Assembly;
        var resource = assembly.GetManifestResourceStream(name);
        if (resource == null)
        {
            throw new Exception("Resource not found: " + name);
        }

        return new StreamReader(resource, Encoding.UTF8).ReadToEnd();
    }

    private static string ExportFunctions(string code)
    {
        var lines = code.Split(Environment.NewLine);
        var exportedLines = lines.Select(line =>
        {
            if (line.StartsWith("function "))
            {
                return "export " + line;
            }

            return line;
        });
        return string.Join(Environment.NewLine, exportedLines);
    }

    private static string EnsureTrailingNewLine(string code)
    {
        return code.Length == 0 ? code : code.TrimEnd() + Environment.NewLine;
    }

    private List<string> GetSerializerAssumptionErrors()
    {
        return new CodeGenSerializerAssumptionsValidator(jsonSerializerOptions, referenceHandlerConfiguration)
            .Validate(_controllers)
            .ToList();
    }

    public TypeScriptApiResult CompileTypeScriptApi(bool generateSwr, bool split, string configFilePath, string? tag)
    {
        var serializerAssumptionErrors = GetSerializerAssumptionErrors();
        if (serializerAssumptionErrors.Count > 0)
        {
            return new TypeScriptApiResult
            {
                TypeScriptApi = "",
                Files = [],
                ErrorMessages = serializerAssumptionErrors
            };
        }

        var builder = new StringBuilder();
        var files = new List<TypeScriptApiFile>();
        string? currentFileName = null;

        void EndFile()
        {
            if (!split || currentFileName == null)
            {
                return;
            }

            files.Add(new TypeScriptApiFile
            {
                FileName = currentFileName,
                Content = EnsureTrailingNewLine(builder.ToString())
            });

            builder.Clear();
            currentFileName = null;
        }

        void BeginFile(string fileName)
        {
            if (!split)
            {
                return;
            }

            EndFile();
            currentFileName = fileName;
            if (generateSwr)
            {
                builder.AppendLine(GetResourceString("CodeGen.Generation.TypeScriptTemplates.header-swr.ts"));
            }
            else
            {
                builder.AppendLine(GetResourceString("CodeGen.Generation.TypeScriptTemplates.header.ts"));
            }
        }

        var result = GenerateTypeScriptApi(generateSwr, split, tag);
        var separator = Environment.NewLine + Environment.NewLine;

        if (!split)
        {
            if (generateSwr)
            {
                builder.AppendLine(GetResourceString("CodeGen.Generation.TypeScriptTemplates.header-swr.ts"));
            }
            else
            {
                builder.AppendLine(GetResourceString("CodeGen.Generation.TypeScriptTemplates.header.ts"));
            }
        }

        BeginFile("_util.ts");
        builder.AppendLine("import _codeGenConfig from '" + configFilePath + "';");
        builder.AppendLine(GetResourceString("CodeGen.Generation.TypeScriptTemplates.util.ts"));
        builder.AppendLine(referenceHandlerConfiguration.PreserveReferences
            ? "export const _restoreCircularReferences = restoreCircularReferences;"
            : "export const _restoreCircularReferences = (obj: any, _: unknown) => obj;");

        if (generateSwr)
        {
            builder.AppendLine(GetResourceString("CodeGen.Generation.TypeScriptTemplates.util-swr.ts"));
        }

        ISet<string> GetIdentifiersUsed(string code)
        {
            return code.Split(new[] { " ", ",", "[", "]", "|", "(", ")", "<", ">" },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToHashSet();
        }

        string ImportAll(IEnumerable<string> imports, string importFilePath, bool typeOnly)
        {
            var lines = imports.Select(i => "    " + i + "," + Environment.NewLine);
            return "import " + (typeOnly ? "type " : "") +
                   "{" + Environment.NewLine + string.Join("", lines) + "} from '" + importFilePath + "';";
        }

        string ImportTypes(ISet<string> identifiersUsed)
        {
            var used = result.DefinitionNames.Where(name =>
                !PrimitiveTypes.Contains(name) && identifiersUsed.Contains(name));
            return ImportAll(used, "./_types", true);
        }

        string ImportConverters(ISet<string> identifiersUsed)
        {
            var used = result.ConverterNames.Where(identifiersUsed.Contains);
            return ImportAll(used, "./_converters", false);
        }

        string ImportUrlBuilders(ISet<string> identifiersUsed)
        {
            var used = result.UrlBuilderNames.Where(identifiersUsed.Contains);
            return ImportAll(used, "./_url-builders", false);
        }

        var orderedControllers = result.Controllers.OrderBy(c => c.Name).ToList();
        foreach (var controller in orderedControllers)
        {
            BeginFile($"{controller.FileName}.ts");
            if (split)
            {
                builder.AppendLine("import { _createHttp, _createObject, _restoreCircularReferences } from './_util';");
                builder.AppendLine("import type { AxiosRequestConfig as _AxiosRequestConfig } from 'axios';");
                if (generateSwr)
                {
                    builder.AppendLine("import _useSWR, { type SWRConfiguration as _SWRConfiguration } from 'swr';");
                    builder.AppendLine("import { _createSWRMiddleware } from './_util';");
                }

                var identifiersUsed = GetIdentifiersUsed(controller.Script);
                builder.AppendLine(ImportTypes(identifiersUsed));
                builder.AppendLine(ImportConverters(identifiersUsed));
                builder.AppendLine(ImportUrlBuilders(identifiersUsed));
            }

            builder.AppendLine(controller.Script);
        }

        BeginFile("_types.ts");
        builder.AppendLine(string.Join(separator, result.DefinitionCodes));

        BeginFile("_converters.ts");
        var converterCode = string.Join(separator, result.ConverterCodes);
        if (split)
        {
            builder.AppendLine("import { _hasOwnPropertyRef, _hasOwnPropertyValues } from './_util';");
            builder.AppendLine(ImportTypes(GetIdentifiersUsed(converterCode)));
            builder.AppendLine(ExportFunctions(GetResourceString("CodeGen.Generation.TypeScriptTemplates.primitive-converters.ts")));
            builder.AppendLine(ExportFunctions(converterCode));
        }
        else
        {
            builder.AppendLine(GetResourceString("CodeGen.Generation.TypeScriptTemplates.primitive-converters.ts"));
            builder.AppendLine(converterCode);
        }

        BeginFile("_url-builders.ts");
        var urlBuildersCode = string.Join(separator, result.UrlBuilderCodes);
        if (split)
        {
            builder.AppendLine(ImportConverters(GetIdentifiersUsed(urlBuildersCode)));
            builder.AppendLine(ExportFunctions(urlBuildersCode));
        }
        else
        {
            builder.AppendLine(urlBuildersCode);
        }

        if (split)
        {
            BeginFile("index.ts");
            builder.AppendLine("export * from './_types';");
            builder.AppendLine("export * from './_util';");
            foreach (var controller in orderedControllers)
            {
                builder.AppendLine(
                    $"export * as {controller.Name} from './{controller.FileName}';");
            }
        }

        EndFile();

        return new TypeScriptApiResult
        {
            TypeScriptApi = split ? "" : EnsureTrailingNewLine(builder.ToString()),
            Files = files,
            ErrorMessages = _errorMessages.Distinct().OrderBy(message => message).ToList()
        };
    }
}
