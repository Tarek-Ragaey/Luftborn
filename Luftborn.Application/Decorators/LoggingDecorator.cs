using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Luftborn.Application.Decorators
{
    public abstract class LoggingDecorator<TService>
    {
        protected readonly TService _service;
        protected readonly ILogger<TService> _logger;

        protected LoggingDecorator(TService service, ILogger<TService> logger)
        {
            _service = service;
            _logger = logger;
        }

        protected void LogMethodEntry(string methodName, params object[] parameters)
        {
            var parameterInfo = parameters.Select((p, i) => $"param{i + 1}: {SerializeParameter(p)}");
            _logger.LogInformation(
                "Entering {ServiceName}.{MethodName} with parameters: {Parameters}",
                typeof(TService).Name,
                methodName,
                string.Join(", ", parameterInfo)
            );
        }

        protected void LogMethodExit(string methodName, object result)
        {
            _logger.LogInformation(
                "Exiting {ServiceName}.{MethodName} with result: {Result}",
                typeof(TService).Name,
                methodName,
                SerializeParameter(result)
            );
        }

        protected void LogError(string methodName, Exception ex, params object[] parameters)
        {
            var parameterInfo = parameters.Select((p, i) => $"param{i + 1}: {SerializeParameter(p)}");
            _logger.LogError(
                ex,
                "Error in {ServiceName}.{MethodName} with parameters: {Parameters}",
                typeof(TService).Name,
                methodName,
                string.Join(", ", parameterInfo)
            );
        }

        private string SerializeParameter(object param)
        {
            if (param == null) return "null";
            
            try
            {
                // Handle sensitive data
                if (IsSensitiveType(param.GetType()))
                    return "[REDACTED]";

                return JsonSerializer.Serialize(param, new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    MaxDepth = 2 // Limit depth to prevent circular references
                });
            }
            catch
            {
                return param.ToString() ?? "null";
            }
        }

        private bool IsSensitiveType(Type type)
        {
            var sensitiveTypes = new[]
            {
                "Password",
                "Token",
                "Secret",
                "Key",
                "Credential"
            };

            return sensitiveTypes.Any(s => type.Name.Contains(s, StringComparison.OrdinalIgnoreCase));
        }
    }
} 