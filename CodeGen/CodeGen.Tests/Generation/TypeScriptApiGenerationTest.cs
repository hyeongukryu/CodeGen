using System.Text.RegularExpressions;
using CodeGen.Analysis;
using CodeGen.Example.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using BuiltInController = CodeGen.Tests.Generation.TestControllers.BuiltIns.BuiltInController;
using CollisionAController = CodeGen.Tests.Generation.TestControllers.CollisionA.CollisionController;
using CollisionBController = CodeGen.Tests.Generation.TestControllers.CollisionB.CollisionController;
using FirstMangleDuplicateController = CodeGen.Tests.Generation.TestControllers.Mangle.First.DuplicateController;
using HttpCallSignatureController = CodeGen.Tests.Generation.TestControllers.HttpCallSignatures.HttpCallSignatureController;
using SecondMangleDuplicateController = CodeGen.Tests.Generation.TestControllers.Mangle.Second.DuplicateController;
using UnsupportedHttpCallSignatureController = CodeGen.Tests.Generation.TestControllers.HttpCallSignatures.UnsupportedHttpCallSignatureController;
using InvalidDefinitionController = CodeGen.Tests.Generation.TestControllers.Invalid.InvalidDefinitionController;
using PrefixASharedDuplicateController = CodeGen.Tests.Generation.TestControllers.PrefixA.Shared.DuplicateController;
using PrefixBSharedDuplicateController = CodeGen.Tests.Generation.TestControllers.PrefixB.Shared.DuplicateController;
using ReservedNameController = CodeGen.Tests.Generation.TestControllers.ReservedNames.ReservedNameController;

namespace CodeGen.Tests.Generation;

public class TypeScriptApiGenerationTest
{
    [Fact]
    public void CompileTypeScriptApi_GeneratesSingleFileWithoutSwr()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(
            configureExpectedJsonOptions: true,
            typeof(OrganizationsController),
            typeof(WeatherForecastController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(false, false, "./config", null);

        Assert.Empty(result.ErrorMessages);
        Assert.Contains("export const Organizations = {", result.TypeScriptApi);
        Assert.Contains("async getAll(", result.TypeScriptApi);
        Assert.Contains("async echo(", result.TypeScriptApi);
        Assert.DoesNotContain("useSWR", result.TypeScriptApi);
        Assert.DoesNotContain("__CODEGEN_VERSION_2_FILE_BOUNDARY__", result.TypeScriptApi);
        Assert.EndsWith(Environment.NewLine, result.TypeScriptApi);
    }

    [Fact]
    public void CompileTypeScriptApi_GeneratesSplitSwrApiAndAppliesTagFilter()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(
            configureExpectedJsonOptions: true,
            typeof(OrganizationsController),
            typeof(WeatherForecastController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(true, true, "./config", "TagB");
        var fileMap = result.Files.ToDictionary(file => file.FileName, file => file.Content);
        var combined = string.Join(Environment.NewLine, fileMap.Values);

        Assert.Empty(result.ErrorMessages);
        Assert.Equal(string.Empty, result.TypeScriptApi);
        Assert.Contains("_util.ts", fileMap.Keys);
        Assert.Contains("index.ts", fileMap.Keys);
        Assert.Contains("import _useSWR", combined);
        Assert.Contains("async function $tagATagB(", combined);
        Assert.Contains("function $useSWRTagATagB(", combined);
        Assert.Contains("export { $tagATagB as tagATagB };", combined);
        Assert.Contains("export { $useSWRTagATagB as useSWRTagATagB };", combined);
        Assert.DoesNotContain("export { $tagA as tagA };", combined);
        Assert.DoesNotContain("export { $getAll as getAll };", combined);
        Assert.All(result.Files, file => Assert.EndsWith(Environment.NewLine, file.Content));
    }

    [Fact]
    public void CompileTypeScriptApi_UsesExportAliasesForSplitControllerFunctions()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(
            configureExpectedJsonOptions: true,
            typeof(ReservedNameController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(false, true, "./config", null);

        Assert.Empty(result.ErrorMessages);
        var controllerFile = result.Files.Single(file =>
            file.FileName.EndsWith("ReservedNameController.ts", StringComparison.Ordinal)).Content;
        Assert.Contains("async function $delete(", controllerFile);
        Assert.Contains("export { $delete as delete };", controllerFile);
        Assert.DoesNotContain("export async function delete(", controllerFile);
        Assert.DoesNotContain("/* delete */", controllerFile);
    }

    [Fact]
    public void CompileTypeScriptApi_DoesNotUseImplementationNameEscapeForSingleFileControllerMethods()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(
            configureExpectedJsonOptions: true,
            typeof(ReservedNameController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(false, false, "./config", null);

        Assert.Empty(result.ErrorMessages);
        Assert.Contains("export const ReservedName = {", result.TypeScriptApi);
        Assert.Contains("async delete(", result.TypeScriptApi);
        Assert.DoesNotContain("$delete", result.TypeScriptApi);
        Assert.DoesNotContain("export { $delete as delete };", result.TypeScriptApi);
        Assert.DoesNotContain("/* delete */", result.TypeScriptApi);
    }

    [Fact]
    public void CompileTypeScriptApi_FailsWhenJsonSerializerOptionsAssumptionsAreViolated()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(
            configureExpectedJsonOptions: false,
            typeof(OrganizationsController),
            typeof(WeatherForecastController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(false, false, "./config", null);

        Assert.Equal(string.Empty, result.TypeScriptApi);
        Assert.Contains(result.ErrorMessages, message => message.Contains("NumberHandling"));
        Assert.Contains(result.ErrorMessages, message => message.Contains("ReferenceHandler"));
    }

    [Fact]
    public void CompileTypeScriptApi_GeneratesBidirectionalConvertersForBuiltInTypes()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(
            configureExpectedJsonOptions: true,
            typeof(BuiltInController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(false, false, "./config", null);

        Assert.Empty(result.ErrorMessages);

        var code = result.TypeScriptApi;
        var webInterface = GetInterfaceBody(code, "BuiltInDto");
        var payloadInterface = GetInterfaceBody(code, "_api_BuiltInDto");

        var webProperties = new (string PropertyName, string TypeName)[]
        {
            ("stringValue", "string"),
            ("booleanValue", "boolean"),
            ("byteValue", "number"),
            ("signedByteValue", "number"),
            ("int16Value", "number"),
            ("int32Value", "number"),
            ("int64Value", "bigint"),
            ("unsignedInt16Value", "number"),
            ("unsignedInt32Value", "number"),
            ("unsignedInt64Value", "bigint"),
            ("singleValue", "number"),
            ("doubleValue", "number"),
            ("decimalValue", "number"),
            ("dateTimeValue", "string"),
            ("dateTimeOffsetValue", "string"),
            ("dateOnlyValue", "string"),
            ("timeOnlyValue", "string"),
            ("guidValue", "string"),
            ("uriValue", "string"),
            ("instantValue", "_Dayjs"),
            ("localDateValue", "string"),
            ("localTimeValue", "string"),
            ("localDateTimeValue", "string"),
            ("bytes", "Uint8Array")
        };
        foreach (var (propertyName, typeName) in webProperties)
        {
            Assert.Contains($"    {propertyName}: {typeName};", webInterface);
        }

        var payloadProperties = webProperties.Select(property =>
            property.PropertyName == "booleanValue"
                ? property
                : (property.PropertyName, "string"));
        foreach (var (propertyName, typeName) in payloadProperties)
        {
            Assert.Contains($"    {propertyName}: {typeName};", payloadInterface);
        }

        Assert.Contains("_convert_BuiltInDto_TO__api_BuiltInDto(request)", code);
        Assert.Contains("_convert__api_BuiltInDto_TO_BuiltInDto(_response.data)", code);

        var clientToServerConverters = new (string PropertyName, string ConverterName)[]
        {
            ("stringValue", "_convert_string_TO_string"),
            ("booleanValue", "_convert_boolean_TO_boolean"),
            ("byteValue", "_convert_number_TO_string"),
            ("signedByteValue", "_convert_number_TO_string"),
            ("int16Value", "_convert_number_TO_string"),
            ("int32Value", "_convert_number_TO_string"),
            ("int64Value", "_convert_bigint_TO_string"),
            ("unsignedInt16Value", "_convert_number_TO_string"),
            ("unsignedInt32Value", "_convert_number_TO_string"),
            ("unsignedInt64Value", "_convert_bigint_TO_string"),
            ("singleValue", "_convert_number_TO_string"),
            ("doubleValue", "_convert_number_TO_string"),
            ("decimalValue", "_convert_number_TO_string"),
            ("dateTimeValue", "_convert_string_TO_string"),
            ("dateTimeOffsetValue", "_convert_string_TO_string"),
            ("dateOnlyValue", "_convert_string_TO_string"),
            ("timeOnlyValue", "_convert_string_TO_string"),
            ("guidValue", "_convert_string_TO_string"),
            ("uriValue", "_convert_string_TO_string"),
            ("instantValue", "_convert__Dayjs_TO_string"),
            ("localDateValue", "_convert_string_TO_string"),
            ("localTimeValue", "_convert_string_TO_string"),
            ("localDateTimeValue", "_convert_string_TO_string"),
            ("bytes", "_convert_Uint8Array_TO_string")
        };
        foreach (var (propertyName, converterName) in clientToServerConverters)
        {
            Assert.Contains($"{propertyName}: {converterName}(from.{propertyName}),", code);
        }

        var serverToClientConverters = clientToServerConverters.Select(converter =>
            converter.ConverterName switch
            {
                "_convert_number_TO_string" => (converter.PropertyName, "_convert_string_TO_number"),
                "_convert_bigint_TO_string" => (converter.PropertyName, "_convert_string_TO_bigint"),
                "_convert__Dayjs_TO_string" => (converter.PropertyName, "_convert_string_TO__Dayjs"),
                "_convert_Uint8Array_TO_string" => (converter.PropertyName, "_convert_string_TO_Uint8Array"),
                _ => converter
            });
        foreach (var (propertyName, converterName) in serverToClientConverters)
        {
            Assert.Contains($"{propertyName}: {converterName}(from.{propertyName}),", code);
        }

        Assert.DoesNotContain("export interface Guid {", code);
        Assert.DoesNotContain("export interface Uri {", code);
        Assert.DoesNotContain("export interface Byte {", code);
        Assert.DoesNotContain("export interface Uint8Array {", code);
    }

    [Fact]
    public void CompileTypeScriptApi_GeneratesAxiosCallSignaturesByHttpMethodAndBody()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(
            configureExpectedJsonOptions: true,
            typeof(HttpCallSignatureController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(false, false, "./config", null);

        Assert.Empty(result.ErrorMessages);
        Assert.Contains(
            "await _createHttp().post(_HttpCallSignature_POST_PostNoBody_url(), undefined, _axiosRequestConfig);",
            result.TypeScriptApi);
        Assert.Contains(
            "await _createHttp().put(_HttpCallSignature_PUT_PutNoBody_url(), undefined, _axiosRequestConfig);",
            result.TypeScriptApi);
        Assert.Contains(
            "await _createHttp().patch(_HttpCallSignature_PATCH_PatchNoBody_url(), undefined, _axiosRequestConfig);",
            result.TypeScriptApi);
        Assert.Contains(
            "await _createHttp().delete(_HttpCallSignature_DELETE_DeleteNoBody_url(), _axiosRequestConfig);",
            result.TypeScriptApi);
    }

    [Fact]
    public void CompileTypeScriptApi_ReportsBodyParametersForUnsupportedHttpMethods()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(
            configureExpectedJsonOptions: true,
            typeof(UnsupportedHttpCallSignatureController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(false, false, "./config", null);

        Assert.Contains(result.ErrorMessages,
            message => message.Contains(
                "BodyParameterUnsupportedHttpMethod DELETE UnsupportedHttpCallSignature DeleteWithBody"));
    }

    [Fact]
    public void CompileTypeScriptApi_AddsSuffixWhenDefinitionNamesCollide()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(configureExpectedJsonOptions: true,
            typeof(CollisionAController),
            typeof(CollisionBController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(false, false, "./config", null);

        Assert.Empty(result.ErrorMessages);
        Assert.Contains("export interface CollisionADuplicateDto {", result.TypeScriptApi);
        Assert.Contains("export interface CollisionBDuplicateDto {", result.TypeScriptApi);
        Assert.Contains("export interface _api_CollisionADuplicateDto {", result.TypeScriptApi);
        Assert.Contains("export interface _api_CollisionBDuplicateDto {", result.TypeScriptApi);
    }

    [Fact]
    public void CompileTypeScriptApi_UsesNamespaceExpansionWhenControllerNamesCollide()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(configureExpectedJsonOptions: true,
            typeof(PrefixASharedDuplicateController),
            typeof(PrefixBSharedDuplicateController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(false, false, "./config", null);

        Assert.Empty(result.ErrorMessages);
        Assert.Contains("export const PrefixASharedDuplicate = {", result.TypeScriptApi);
        Assert.Contains("export const PrefixBSharedDuplicate = {", result.TypeScriptApi);
        Assert.DoesNotContain("export const SharedDuplicate = {", result.TypeScriptApi);
    }

    [Fact]
    public void CompileTypeScriptApi_UsesMangledSuffixWhenNamespaceExpansionCannotDisambiguateNames()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(configureExpectedJsonOptions: true,
            typeof(FirstMangleDuplicateController),
            typeof(SecondMangleDuplicateController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(false, false, "./config", null);

        Assert.Empty(result.ErrorMessages);
        AssertMangledNames(
            result.TypeScriptApi,
            @"export const (?<name>CodeGenTestsGenerationTestControllersMangleDuplicate_M[0-9a-f]+) = \{");
        AssertMangledNames(
            result.TypeScriptApi,
            @"export interface (?<name>CodeGenTestsGenerationTestControllersMangleDuplicateDto_M[0-9a-f]+) \{");
        AssertMangledNames(
            result.TypeScriptApi,
            @"export interface (?<name>_api_CodeGenTestsGenerationTestControllersMangleDuplicateDto_M[0-9a-f]+) \{");
        Assert.DoesNotContain("export const CodeGenTestsGenerationTestControllersMangleDuplicate = {",
            result.TypeScriptApi);
        Assert.DoesNotContain("export interface CodeGenTestsGenerationTestControllersMangleDuplicateDto {",
            result.TypeScriptApi);
    }

    [Fact]
    public void CompileTypeScriptApi_UsesReadableTypeIdentityBasedFileNamesForSplitOutput()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(configureExpectedJsonOptions: true,
            typeof(PrefixASharedDuplicateController),
            typeof(PrefixBSharedDuplicateController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(false, true, "./config", null);
        var fileMap = result.Files.ToDictionary(file => file.FileName, file => file.Content);

        Assert.Empty(result.ErrorMessages);
        Assert.Equal(string.Empty, result.TypeScriptApi);
        Assert.Contains(
            "CodeGen.Tests.Generation.TestControllers.PrefixA.Shared.DuplicateController.ts",
            fileMap.Keys);
        Assert.Contains(
            "CodeGen.Tests.Generation.TestControllers.PrefixB.Shared.DuplicateController.ts",
            fileMap.Keys);
        Assert.Contains(
            "export * as PrefixASharedDuplicate from './CodeGen.Tests.Generation.TestControllers.PrefixA.Shared.DuplicateController';",
            fileMap["index.ts"]);
        Assert.Contains(
            "export * as PrefixBSharedDuplicate from './CodeGen.Tests.Generation.TestControllers.PrefixB.Shared.DuplicateController';",
            fileMap["index.ts"]);
    }

    [Fact]
    public void CompileTypeScriptApi_ReportsInvalidControllerDefinitions()
    {
        using var serviceProvider = GenerationTestHelper.BuildServiceProvider(
            configureExpectedJsonOptions: true,
            typeof(InvalidDefinitionController));

        var analyzer = serviceProvider.GetRequiredService<ApiAnalyzer>();
        var result = analyzer.Analyze().CompileTypeScriptApi(false, false, "./config", null);

        Assert.Contains(result.ErrorMessages, message => message.Contains("SupportedResponseTypes InvalidDefinition InvalidResponse"));
    }

    private static void AssertMangledNames(string text, string pattern)
    {
        var names = Regex.Matches(text, pattern)
            .Select(match => match.Groups["name"].Value)
            .Distinct()
            .ToList();

        Assert.Equal(2, names.Count);
    }

    private static string GetInterfaceBody(string code, string interfaceName)
    {
        var match = Regex.Match(code,
            @$"export interface {Regex.Escape(interfaceName)} \{{(?<body>.*?)\n\}}",
            RegexOptions.Singleline);

        Assert.True(match.Success, $"Interface {interfaceName} was not generated.");
        return match.Groups["body"].Value;
    }
}
