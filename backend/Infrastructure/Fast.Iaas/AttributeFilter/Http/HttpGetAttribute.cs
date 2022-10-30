#nullable enable
namespace Fast.Iaas.AttributeFilter.Http;

/// <summary>
/// Identifies an action that supports the HTTP GET method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HttpGetAttribute : HttpMethodAttribute
{
    private static readonly IEnumerable<string> _supportedMethods = new[] {"GET"};

    /// <summary>
    /// Interface/method operation name
    /// </summary>
    public string? OperationName { get; }

    /// <summary>
    /// Creates a new <see cref="HttpGetAttribute"/>.
    /// </summary>
    public HttpGetAttribute() : base(_supportedMethods)
    {
    }

    /// <summary>
    /// Creates a new <see cref="HttpGetAttribute"/> with the given route template.
    /// </summary>
    /// <param name="template">The route template. May not be null.</param>
    public HttpGetAttribute(string template) : base(_supportedMethods, template)
    {
        if (template == null)
        {
            throw new ArgumentNullException(nameof(template));
        }
    }

    /// <summary>
    /// Creates a new <see cref="HttpGetAttribute"/> with the given route template.
    /// </summary>
    /// <param name="template">The route template. May not be null.</param>
    /// <param name="operationName">Interface/method operation name. May not be null.</param>
    public HttpGetAttribute(string template, string operationName) : base(_supportedMethods, template)
    {
        if (template == null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        OperationName = operationName ?? throw new ArgumentNullException(nameof(operationName));
    }
}