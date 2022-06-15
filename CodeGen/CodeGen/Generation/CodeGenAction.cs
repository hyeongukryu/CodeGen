using Microsoft.AspNetCore.Mvc.Controllers;

namespace CodeGen.Generation;

public class CodeGenAction
{
    public CodeGenAction(CodeGenController controller, string name, string template, string httpMethod,
        ControllerParameterDescriptor? bodyParameter, IEnumerable<ControllerParameterDescriptor> pathParameters,
        IEnumerable<ControllerParameterDescriptor> queryParameters, Type? responseType, bool isCommand)
    {
        Controller = controller;
        Name = name;
        Template = template;
        HttpMethod = httpMethod;
        BodyParameter = bodyParameter;
        PathParameters = pathParameters;
        QueryParameters = queryParameters;
        ResponseType = responseType;
        IsCommand = isCommand;
    }

    public CodeGenController Controller { get; }
    public string Name { get; }
    public string Template { get; }
    public string HttpMethod { get; }
    public ControllerParameterDescriptor? BodyParameter { get; }
    public IEnumerable<ControllerParameterDescriptor> PathParameters { get; }
    public IEnumerable<ControllerParameterDescriptor> QueryParameters { get; }
    public Type? ResponseType { get; }
    public bool IsCommand { get; }
}