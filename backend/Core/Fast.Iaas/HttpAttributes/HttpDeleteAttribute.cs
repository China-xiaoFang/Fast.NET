#nullable enable

using Fast.Iaas.Internal;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Fast.Iaas.HttpAttributes;

/// <summary>
/// Identifies an action that supports the HTTP DELETE method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HttpDeleteAttribute : HttpMethodAttribute
{
    private static readonly IEnumerable<string> _supportedMethods = new[] {"DELETE"};

    /// <summary>
    /// Interface/method operation name
    /// </summary>
    public string? OperationName { get; }

    /// <summary>
    /// Interface/method operation action
    /// </summary>
    public HttpRequestActionEnum? Action { get; }

    /// <summary>
    /// Creates a new <see cref="HttpDeleteAttribute"/>.
    /// </summary>
    public HttpDeleteAttribute() : base(_supportedMethods)
    {
    }

    /// <summary>
    /// Creates a new <see cref="HttpDeleteAttribute"/> with the given route template.
    /// </summary>
    /// <param name="template">The route template. May not be null.</param>
    public HttpDeleteAttribute(string template) : base(_supportedMethods, template)
    {
        if (template == null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        Action = HttpRequestActionEnum.Delete;
    }

    /// <summary>
    /// Creates a new <see cref="HttpDeleteAttribute"/> with the given route template.
    /// </summary>
    /// <param name="template">The route template. May not be null.</param>
    /// <param name="operationName">Interface/method operation name. May not be null.</param>
    public HttpDeleteAttribute(string template, string operationName) : base(_supportedMethods, template)
    {
        if (template == null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        OperationName = operationName ?? throw new ArgumentNullException(nameof(operationName));
        Action = HttpRequestActionEnum.Delete;
    }

    /// <summary>
    /// Creates a new <see cref="HttpDeleteAttribute"/> with the given route template.
    /// </summary>
    /// <param name="template">The route template. May not be null.</param>
    /// <param name="operationName">Interface/method operation name. May not be null.</param>
    /// <param name="action">Interface/method operation action. May not be null.</param>
    public HttpDeleteAttribute(string template, string operationName, HttpRequestActionEnum action) : base(_supportedMethods,
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