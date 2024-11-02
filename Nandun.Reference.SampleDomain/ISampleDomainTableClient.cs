using System.Threading;
using System.Threading.Tasks;

namespace Nandun.Reference.SampleDomain;

/// <summary>
/// Allows you to interact with the Storage Table that contains  sample Data
/// </summary>
public interface ISampleDomainTableClient
{
    /// <summary>
    /// Gets single <see cref="ISampleDomain"/> for a given
    /// <see>
    ///     <cref>orderFulfillmentCode</cref>
    /// </see>
    /// and
    /// <see>
    ///     <cref>fulfillmentReferenceNumber</cref>
    /// </see>
    /// </summary>
    /// <param name="partitionKey"></param>
    /// <param name="rowKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ISampleDomain> GetAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);

    /// <summary>
    /// Inserts or updates a <see cref="ISampleDomain"/>
    /// </summary>
    /// <param name="sample"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task SetAsync(ISampleDomain sample, CancellationToken cancellationToken);
}