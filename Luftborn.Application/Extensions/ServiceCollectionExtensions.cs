using Microsoft.Extensions.DependencyInjection;
using Luftborn.Application.Decorators;
using Luftborn.Application.IServices.Interfaces;
using Luftborn.Application.Services.Services;
using Microsoft.Extensions.Logging;

namespace Luftborn.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register services with their decorators
            services.AddScoped<IArticleService, ArticleService>();
            services.Decorate<IArticleService, ArticleServiceLoggingDecorator>();

            // Add other service registrations here
            return services;
        }

        private static IServiceCollection Decorate<TService, TDecorator>(
            this IServiceCollection services)
            where TService : class
            where TDecorator : class, TService
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
            if (descriptor == null)
                throw new InvalidOperationException($"Service of type {typeof(TService).Name} is not registered");

            var serviceType = typeof(TService);
            var decoratorType = typeof(TDecorator);
            var innerType = descriptor.ImplementationType ?? descriptor.ServiceType;

            var innerDescriptor = ServiceDescriptor.Describe(
                innerType,
                innerType,
                descriptor.Lifetime);

            var decoratorDescriptor = ServiceDescriptor.Describe(
                serviceType,
                sp =>
                {
                    var inner = sp.GetRequiredService(innerType);
                    var logger = sp.GetRequiredService(typeof(ILogger<>).MakeGenericType(serviceType));
                    return ActivatorUtilities.CreateInstance(sp, decoratorType, inner, logger);
                },
                descriptor.Lifetime);

            services.Remove(descriptor);
            services.Add(innerDescriptor);
            services.Add(decoratorDescriptor);

            return services;
        }
    }
} 