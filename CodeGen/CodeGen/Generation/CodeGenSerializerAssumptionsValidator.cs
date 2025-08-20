using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using CodeGen.Analysis;
using NodaTime;

namespace CodeGen.Generation;

internal sealed class CodeGenSerializerAssumptionsValidator(
    JsonSerializerOptions jsonSerializerOptions,
    IReferenceHandlerConfiguration referenceHandlerConfiguration)
{
    private readonly NullabilityInfoContext _nullabilityInfoContext = new();
    private readonly ISet<Type> _validatedTypes = new HashSet<Type>();

    public IEnumerable<string> Validate(IEnumerable<CodeGenController> controllers)
    {
        var errorMessages = new List<string>();

        ValidateGlobalOptions(errorMessages);

        foreach (var action in controllers.SelectMany(controller => controller.Actions))
        {
            if (action.ResponseType != null)
            {
                ValidateType(action.ResponseType, errorMessages);
            }

            if (action.BodyParameter != null)
            {
                ValidateParameter(action.BodyParameter.ParameterInfo, errorMessages);
            }

            foreach (var parameter in action.PathParameters.Concat(action.QueryParameters))
            {
                ValidateParameter(parameter.ParameterInfo, errorMessages);
            }
        }

        return errorMessages.Distinct().OrderBy(message => message).ToList();
    }

    private void ValidateGlobalOptions(ICollection<string> errorMessages)
    {
        if (!UsesCamelCasePropertyNamingPolicy())
        {
            errorMessages.Add(
                "JsonSerializerOptions.PropertyNamingPolicy must be camelCase for CodeGen generation.");
        }

        if (jsonSerializerOptions.DefaultIgnoreCondition != JsonIgnoreCondition.Never)
        {
            errorMessages.Add(
                "JsonSerializerOptions.DefaultIgnoreCondition must be Never for CodeGen generation.");
        }

        if (!jsonSerializerOptions.NumberHandling.HasFlag(JsonNumberHandling.WriteAsString) ||
            !jsonSerializerOptions.NumberHandling.HasFlag(JsonNumberHandling.AllowReadingFromString))
        {
            errorMessages.Add(
                "JsonSerializerOptions.NumberHandling must include WriteAsString and AllowReadingFromString for CodeGen generation.");
        }

        var usesPreserveReferences = ReferenceEquals(jsonSerializerOptions.ReferenceHandler, ReferenceHandler.Preserve);
        if (referenceHandlerConfiguration.PreserveReferences != usesPreserveReferences)
        {
            errorMessages.Add(
                "JsonSerializerOptions.ReferenceHandler must match the AddCodeGen preserveReferences setting.");
        }
    }

    private bool UsesCamelCasePropertyNamingPolicy()
    {
        return jsonSerializerOptions.PropertyNamingPolicy?.ConvertName("SamplePropertyName") == "samplePropertyName";
    }

    private void ValidateParameter(ParameterInfo parameterInfo, ICollection<string> errorMessages)
    {
        ValidateUnsupportedParameterAttributes(parameterInfo, errorMessages);
        ValidateType(parameterInfo.ParameterType, errorMessages);
    }

    private void ValidateType(Type type, ICollection<string> errorMessages)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        if (!_validatedTypes.Add(underlyingType))
        {
            return;
        }

        if (IsSimpleType(underlyingType))
        {
            ValidateSpecialTypeSerialization(underlyingType, errorMessages);
            return;
        }

        if (TryGetDictionaryValueType(underlyingType, out var valueType))
        {
            ValidateType(valueType, errorMessages);
            return;
        }

        if (underlyingType.IsEnumerable())
        {
            var elementType = underlyingType.GetEnumerableElementType();
            if (elementType != null)
            {
                ValidateType(elementType, errorMessages);
            }

            return;
        }

        if (underlyingType.GetCustomAttribute<JsonConverterAttribute>() != null)
        {
            errorMessages.Add(
                $"Custom JsonConverter attributes are not supported for DTO type '{underlyingType.FullName}'.");
        }

        if (underlyingType.GetCustomAttribute<JsonDerivedTypeAttribute>() != null ||
            underlyingType.GetCustomAttributes<JsonDerivedTypeAttribute>().Any() ||
            underlyingType.GetCustomAttribute<JsonPolymorphicAttribute>() != null)
        {
            errorMessages.Add(
                $"Polymorphic JSON serialization is not supported for DTO type '{underlyingType.FullName}'.");
        }

        foreach (var propertyInfo in underlyingType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (propertyInfo.GetMethod == null || propertyInfo.GetMethod.GetParameters().Length > 0)
            {
                continue;
            }

            if (propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>()?.Condition == JsonIgnoreCondition.Always)
            {
                continue;
            }

            ValidateUnsupportedPropertyAttributes(propertyInfo, errorMessages, underlyingType.FullName ?? underlyingType.Name);
            ValidatePropertyNullability(propertyInfo, errorMessages);
            ValidateType(propertyInfo.PropertyType, errorMessages);
        }
    }

    private void ValidateUnsupportedParameterAttributes(ParameterInfo parameterInfo, ICollection<string> errorMessages)
    {
        if (parameterInfo.GetCustomAttribute<JsonPropertyNameAttribute>() != null)
        {
            errorMessages.Add(
                $"JsonPropertyName is not supported for parameter '{parameterInfo.Name}'.");
        }

        var jsonIgnore = parameterInfo.GetCustomAttribute<JsonIgnoreAttribute>();
        if (jsonIgnore != null && jsonIgnore.Condition != JsonIgnoreCondition.Always)
        {
            errorMessages.Add(
                $"Conditional JsonIgnore is not supported for parameter '{parameterInfo.Name}'.");
        }

        if (parameterInfo.GetCustomAttribute<JsonConverterAttribute>() != null)
        {
            errorMessages.Add(
                $"Custom JsonConverter attributes are not supported for parameter '{parameterInfo.Name}'.");
        }
    }

    private void ValidateUnsupportedPropertyAttributes(MemberInfo memberInfo, ICollection<string> errorMessages, string ownerName)
    {
        if (memberInfo.GetCustomAttribute<JsonPropertyNameAttribute>() != null)
        {
            errorMessages.Add(
                $"JsonPropertyName is not supported for '{ownerName}.{memberInfo.Name}'.");
        }

        var jsonIgnore = memberInfo.GetCustomAttribute<JsonIgnoreAttribute>();
        if (jsonIgnore != null && jsonIgnore.Condition != JsonIgnoreCondition.Always)
        {
            errorMessages.Add(
                $"Conditional JsonIgnore is not supported for '{ownerName}.{memberInfo.Name}'.");
        }

        if (memberInfo.GetCustomAttribute<JsonConverterAttribute>() != null)
        {
            errorMessages.Add(
                $"Custom JsonConverter attributes are not supported for '{ownerName}.{memberInfo.Name}'.");
        }
    }

    private void ValidatePropertyNullability(PropertyInfo propertyInfo, ICollection<string> errorMessages)
    {
        if (!propertyInfo.PropertyType.IsEnumerable())
        {
            return;
        }

        var nullabilityInfo = _nullabilityInfoContext.Create(propertyInfo);
        if (nullabilityInfo.ElementType?.ReadState == NullabilityState.Nullable)
        {
            errorMessages.Add(
                $"Nullable collection elements are not supported for '{propertyInfo.DeclaringType?.FullName}.{propertyInfo.Name}'.");
        }
    }

    private void ValidateSpecialTypeSerialization(Type type, ICollection<string> errorMessages)
    {
        if (type == typeof(Instant))
        {
            ValidateRoundTripSerialization(Instant.FromUtc(2024, 1, 2, 3, 4), "NodaTime.Instant", errorMessages);
            return;
        }

        if (type == typeof(LocalDate))
        {
            ValidateRoundTripSerialization(new LocalDate(2024, 1, 2), "NodaTime.LocalDate", errorMessages);
            return;
        }

        if (type == typeof(LocalTime))
        {
            ValidateRoundTripSerialization(new LocalTime(3, 4, 5), "NodaTime.LocalTime", errorMessages);
            return;
        }

        if (type == typeof(LocalDateTime))
        {
            ValidateRoundTripSerialization(new LocalDateTime(2024, 1, 2, 3, 4, 5), "NodaTime.LocalDateTime",
                errorMessages);
        }
    }

    private void ValidateRoundTripSerialization<T>(T sampleValue, string typeName, ICollection<string> errorMessages)
    {
        try
        {
            var json = JsonSerializer.Serialize(sampleValue, jsonSerializerOptions);
            if (!json.StartsWith('"') || !json.EndsWith('"'))
            {
                errorMessages.Add(
                    $"JsonSerializerOptions must serialize {typeName} as a JSON string for CodeGen generation.");
                return;
            }

            var roundTripped = JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
            if (!EqualityComparer<T>.Default.Equals(sampleValue, roundTripped))
            {
                errorMessages.Add(
                    $"JsonSerializerOptions must deserialize {typeName} values symmetrically for CodeGen generation.");
            }
        }
        catch (Exception ex)
        {
            errorMessages.Add(
                $"JsonSerializerOptions must support {typeName} for CodeGen generation. {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static bool TryGetDictionaryValueType(Type type, out Type valueType)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            valueType = type.GenericTypeArguments[1];
            return true;
        }

        var dictionaryInterface = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        if (dictionaryInterface != null)
        {
            valueType = dictionaryInterface.GenericTypeArguments[1];
            return true;
        }

        valueType = null!;
        return false;
    }

    private static bool IsSimpleType(Type type)
    {
        if (type.IsEnum)
        {
            return true;
        }

        return type.FullName switch
        {
            "System.String" => true,
            "System.Boolean" => true,
            "System.Byte" => true,
            "System.SByte" => true,
            "System.Int16" => true,
            "System.Int32" => true,
            "System.Int64" => true,
            "System.UInt16" => true,
            "System.UInt32" => true,
            "System.UInt64" => true,
            "System.Single" => true,
            "System.Double" => true,
            "System.Decimal" => true,
            "System.DateTime" => true,
            "System.DateTimeOffset" => true,
            "System.DateOnly" => true,
            "System.TimeOnly" => true,
            "System.Guid" => true,
            "System.Uri" => true,
            "NodaTime.Instant" => true,
            "NodaTime.LocalDate" => true,
            "NodaTime.LocalTime" => true,
            "NodaTime.LocalDateTime" => true,
            _ => type == typeof(byte[])
        };
    }
}
