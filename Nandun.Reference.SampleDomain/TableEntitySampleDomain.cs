using System;
using System.Diagnostics.CodeAnalysis;
using Azure;
using Azure.Data.Tables;
using Nandun.Reference.SampleDomain.Extensions;

namespace Nandun.Reference.SampleDomain;

[ExcludeFromCodeCoverage]
internal class TableEntitySampleDomain : ITableEntity, ISampleDomain
{
    /// <summary>
    /// Empty constructor for mapping
    /// </summary>
    public TableEntitySampleDomain()
    {
    }

    /// <summary>
    /// Constructor to initialize with a <see cref="ISampleDomain"/>
    /// </summary>
    /// <param name="SampleDomain"></param>
    public TableEntitySampleDomain(ISampleDomain SampleDomain)
    {
        SampleDomain.MergeTo(this);
    }

    public string PartitionKey {
        get => SampleCode;
        set => SampleCode = value;
    }

    public string RowKey
    {
        get => Id;
        set => Id = value;
    }

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string Id { get; set; }
    public DateTime ProcessingDate { get; set; }
    public string SampleCode { get; set; }
}