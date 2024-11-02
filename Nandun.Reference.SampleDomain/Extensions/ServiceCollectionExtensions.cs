using Nandun.Reference.SampleDomain;

// ReSharper disable once CheckNamespace - MS recommends keeping service collection extensions under this name space
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for dependency configurations.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds support to operate on the DSP Deliveries Table Storage.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static IServiceCollection AddTableStorageSampleDomain(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<ISampleDomainTableClient>(_ => new DefaultSampleDomainTableClient(connectionString));
        return services;
    }

}