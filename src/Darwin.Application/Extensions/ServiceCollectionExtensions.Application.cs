using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.Extensions.Microsoft.DependencyInjection;
using FluentValidation;

namespace Darwin.Application.Extensions
{
    /// <summary>
    /// Registers Application layer services: AutoMapper profiles and FluentValidation validators via assembly scanning.
    /// </summary>
    public static class ServiceCollectionExtensionsApplication
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Two overloads exist. This one avoids ambiguity.
            services.AddAutoMapper(cfg => { /* optional global config */ }, assembly);

            // Requires FluentValidation.DependencyInjectionExtensions package
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
