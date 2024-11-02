using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace Nandun.Reference.SampleDomain;

/// <summary>
/// Default implementation of <see cref="ISampleDomainTableClient"/>
/// </summary>
internal class DefaultSampleDomainTableClient : ISampleDomainTableClient
{
    private const string SAMPLE_TABLE_NAME = "SampleDomain";

    private readonly Func<TableClient> _factory;

    private TableClient _client;

    private readonly int _createTableAttempts;

    /// <summary>
    /// Initialize with connection string
    /// </summary>
    /// <param name="connectionString"></param>
    public DefaultSampleDomainTableClient(string connectionString) :
        this(() => new TableClient(connectionString, SAMPLE_TABLE_NAME))
    {
    }

    /// <summary>
    /// Initialize with factory
    /// </summary>
    /// <param name="factory">factory that creates a <see cref="TableClient"/></param>
    /// <param name="createTableAttempts">amount of times to attempt to create table if it doesn't exist.</param>
    public DefaultSampleDomainTableClient(Func<TableClient> factory, int createTableAttempts = 3)
    {
        _factory = factory;
        _createTableAttempts = createTableAttempts;
    }

    public async Task<ISampleDomain> GetAsync(string orderFulfillmentCode, string fulfillmentReferenceNumber, CancellationToken cancellationToken)
    {
        _client ??= _factory();

        Response<TableEntitySampleDomain> response =
            await _client.GetEntityAsync<TableEntitySampleDomain>(orderFulfillmentCode, fulfillmentReferenceNumber,
                cancellationToken: cancellationToken);

        return response.Value;
    }

    public async Task SetAsync(ISampleDomain sample, CancellationToken cancellationToken)
    {
        _client ??= _factory();
        int attempts = 0;
        do
        {
            try
            {
                await _client.UpsertEntityAsync(new TableEntitySampleDomain(sample),
                    cancellationToken: cancellationToken);
                break;
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == "TableNotFound" && ++attempts <= _createTableAttempts)
            {
                await _client.CreateIfNotExistsAsync(cancellationToken);
            }
        } while (true);
    }
}