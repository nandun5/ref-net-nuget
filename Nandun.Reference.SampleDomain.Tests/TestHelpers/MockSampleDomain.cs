using System;

namespace Nandun.Reference.SampleDomain.Tests.TestHelpers;

internal class MockSampleDomain : ISampleDomain
{
    public string Id { get; set; }
    public DateTime ProcessingDate { get; set; }
    public string SampleCode { get; set; }

    internal static MockSampleDomain CreateMock()
    {
        MockSampleDomain sample = new()
        {
            Id = "MockDrive",
            ProcessingDate = DateTime.UtcNow.Date,
            SampleCode = Guid.NewGuid().ToString()
        };
        return sample;
    }
}

