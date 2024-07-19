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
            context.Request.EnableBuffering(); // Allow request body to be read multiple times
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0; // Reset the position to allow the next middleware to read the body
            _logger.LogInformation($"Incoming request: {context.Request.Method} {context.Request.Path} {requestBody}");

            // Capture and log response details
            var originalBodyStream = context.Response.Body; // Save the original response body stream
            using (var responseBody = new MemoryStream()) // Use a memory stream to capture the response
            {
                context.Response.Body = responseBody; // Replace the response body stream with our memory stream
                try
                {
                    await _next(context); // Invoke the next middleware

                    // Log response details
                    context.Response.Body.Seek(0, SeekOrigin.Begin); // Move to the beginning of the response stream
                    var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin); // Reset the position to allow the response to be sent to the client

                    // Log only if the response content type is JSON or plain text
                    if (context.Response.ContentType != null &&
                        (context.Response.ContentType.Contains("application/json") || context.Response.ContentType.Contains("text/plain")))
                    {
                        _logger.LogInformation($"Response: {context.Response.StatusCode} {responseBodyText}");
                    }

                    await responseBody.CopyToAsync(originalBodyStream); // Copy the response to the original stream
                }
                catch (Exception ex)
                {
                    // Log any exceptions that occur
                    _logger.LogError(ex, "An unhandled exception has occurred while executing the request.");
                    throw; // Re-throw the exception to let the default error handler handle it
                }
            }
        }
    }
}
