using System;

namespace Nandun.Reference.SampleDomain;

/// <summary>
/// Represents a Sample Domain object Table
/// </summary>
public interface ISampleDomain
{
    /// <summary>
    /// Gets or sets the Id
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Gets or sets the  processing date.
    /// </summary>
    DateTime ProcessingDate { get; set; }

    /// <summary>
    /// Gets or sets the Sample Code.
    /// </summary>
    string SampleCode { get; set; }

}