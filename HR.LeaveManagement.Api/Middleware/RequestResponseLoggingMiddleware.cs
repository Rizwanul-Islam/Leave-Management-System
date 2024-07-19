using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Api.Middleware
{
    /// <summary>
    /// Middleware to log request and response details for debugging and monitoring purposes.
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestResponseLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next request delegate in the pipeline.</param>
        /// <param name="logger">The logger to log request and response details.</param>
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware, logs request and response details, and handles exceptions.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        public async Task Invoke(HttpContext context)
        {
            // Skip logging for requests to Swagger endpoints
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // Log request details
            context.Request.EnableBuffering(); 
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0; 
            _logger.LogInformation($"Incoming request: {context.Request.Method} {context.Request.Path} {requestBody}");

            // Capture and log response details
            var originalBodyStream = context.Response.Body; 
            using (var responseBody = new MemoryStream()) 
            {
                context.Response.Body = responseBody; 
                try
                {
                    await _next(context); 

                    // Log response details
                    context.Response.Body.Seek(0, SeekOrigin.Begin); 
                    var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin); 

                    // Log only if the response content type is JSON or plain text
                    if (context.Response.ContentType != null &&
                        (context.Response.ContentType.Contains("application/json") || context.Response.ContentType.Contains("text/plain")))
                    {
                        _logger.LogInformation($"Response: {context.Response.StatusCode} {responseBodyText}");
                    }

                    await responseBody.CopyToAsync(originalBodyStream); 
                }
                catch (Exception ex)
                {
                    // Log any exceptions that occur
                    _logger.LogError(ex, "An unhandled exception has occurred while executing the request.");
                    throw; 
                }
            }
        }
    }
}
