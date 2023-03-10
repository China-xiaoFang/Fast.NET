#nullable enable
using Fast.SDK.Common.EnumFactory;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Fast.SDK.Common.AttributeFilter.Http;

/// <summary>
/// Identifies an action that supports the HTTP POST method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HttpPostAttribute : HttpMethodAttribute
{
    private static readonly IEnumerable<string> _supportedMethods = new[] {"POST"};

    /// <summary>
    /// Interface/method operation name
    /// </summary>
    public string? OperationName { get; }

    /// <summary>
    /// Interface/method operation action
    /// </summary>
    public HttpRequestActionEnum? Action { get; }

    /// <summary>
    /// Creates a new <see cref="HttpPostAttribute"/>.
    /// </summary>
    public HttpPostAttribute() : base(_supportedMethods)
    {
    }

    /// <summary>
    /// Creates a new <see cref="HttpPostAttribute"/> with the given route template.
    /// </summary>
    /// <param name="template">The route template. May not be null.</param>
    public HttpPostAttribute(string template) : base(_supportedMethods, template)
    {
        if (template == null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        Action = HttpRequestActionEnum.Add;
    }

    /// <summary>
    /// Creates a new <see cref="HttpPostAttribute"/> with the given route template.
    /// </summary>
    /// <param name="template">The route template. May not be null.</param>
    /// <param name="operationName">Interface/method operation name. May not be null.</param>
    public HttpPostAttribute(string template, string operationName) : base(_supportedMethods, template)
    {
        if (template == null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        OperationName = operationName ?? throw new ArgumentNullException(nameof(operationName));
        Action = HttpRequestActionEnum.Add;
    }

    /// <summary>
    /// Creates a new <see cref="HttpPostAttribute"/> with the given route template.
    /// </summary>
    /// <param name="template">The route template. May not be null.</param>
    /// <param name="operationName">Interface/method operation name. May not be null.</param>
    /// <param name="action">Interface/method operation action. May not be null.</param>
    public HttpPostAttribute(string template, string operationName, HttpRequestActionEnum action) : base(_supportedMethods,
        template)
    {
        if (template == null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        OperationName = operationName ?? throw new ArgumentNullException(nameof(operationName));
        Action = action;
    }
}