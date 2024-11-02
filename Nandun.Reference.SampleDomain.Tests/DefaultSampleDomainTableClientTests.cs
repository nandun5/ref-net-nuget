using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Nandun.Reference.SampleDomain.Extensions;
using Nandun.Reference.SampleDomain.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Nandun.Reference.SampleDomain.Tests;

[TestClass]
public class DefaultSampleDomainTableClientTests
{
    private Mock<TableClient> _tableClientMock;

    [TestInitialize]
    public void Setup()
    {
        _tableClientMock = new Mock<TableClient>();
    }

    #region Properties

    public TestContext TestContext { get; set; }

    #endregion

    private ISampleDomainTableClient CreateIntegrationTestSubject()
    {
        //HINT: for this to work, ensure the storage account KEY is updated to in the test.runsettings file.
        ServiceCollection services = new();
        services.AddTableStorageSampleDomain(TestContext.Properties["StorageAccountConnectionString"]?.ToString());
        return services.BuildServiceProvider().GetService<ISampleDomainTableClient>();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task IntegrationTest()
    {
        //HINT: for this test to work, ensure the storage account KEY is updated to in the test.runsettings file.
        ISampleDomainTableClient client = CreateIntegrationTestSubject();

        MockSampleDomain newsample = MockSampleDomain.CreateMock();

        await client.SetAsync(newsample, CancellationToken.None);

        ISampleDomain sampleFromTable = await client.GetAsync(newsample.SampleCode, newsample.Id, CancellationToken.None);

        MockSampleDomain existingsample = new();

        sampleFromTable.MergeTo(existingsample);

        Assert.AreEqual(newsample.Id, existingsample.Id);
        Assert.AreEqual(newsample.ProcessingDate, existingsample.ProcessingDate);
        Assert.AreEqual(newsample.SampleCode, existingsample.SampleCode);


    }

    [TestMethod]
    [TestCategory("Unit")]
    public void DefaultSampleDomainTableClient_Ctor_WhenConnectionStringProvided_ReturnsClient()
    {
        DefaultSampleDomainTableClient client =
            new(TestContext.Properties["StorageAccountConnectionString"]?.ToString());

        Assert.IsNotNull(client);
    }


    [TestMethod]
    [TestCategory("Unit")]
    public void DefaultSampleDomainTableClient_Ctor_WhenFactory_ReturnsClient()
    {
        DefaultSampleDomainTableClient client =
            new(() => null);

        Assert.IsNotNull(client);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task DefaultSampleDomainTableClient_GetAsync_WhenInvoked_ReturnsSampleDomain()
    {
        TableEntitySampleDomain sample = new();

        Mock<Response<TableEntitySampleDomain>> response = new();

        response.Setup(m => m.Value).Returns(sample);

        _tableClientMock.Setup(m => m.GetEntityAsync<TableEntitySampleDomain>(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response.Object);

        DefaultSampleDomainTableClient client = new(() => _tableClientMock.Object);

        ISampleDomain result = await client.GetAsync("test", "test", CancellationToken.None);

        Assert.AreSame(sample, result);
    }


    [TestMethod]
    [TestCategory("Unit")]
    public async Task DefaultSampleDomainTableClient_SetAsync_WhenTableExists_WritesToTable()
    {
        ISampleDomain sample = MockSampleDomain.CreateMock();

        Mock<Response> response = new();


        _tableClientMock
            .Setup(m => m.UpsertEntityAsync(It.IsAny<TableEntitySampleDomain>(), It.IsAny<TableUpdateMode>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(response.Object);

        DefaultSampleDomainTableClient client = new(() => _tableClientMock.Object);

        await client.SetAsync(sample, CancellationToken.None);

        _tableClientMock.Verify(m => m.UpsertEntityAsync(It.IsAny<TableEntitySampleDomain>(), It.IsAny<TableUpdateMode>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task DefaultSampleDomainTableClient_SetAsync_WhenTableDoesNotExist_CreatesAndWritesToTable()
    {
        ISampleDomain sample = MockSampleDomain.CreateMock();

        Mock<Response> response = new();

        _tableClientMock.SetupSequence(m => m.UpsertEntityAsync(It.IsAny<TableEntitySampleDomain>(), It.IsAny<TableUpdateMode>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(0, "Mock Message", "TableNotFound", null))
            .ReturnsAsync(response.Object);

        DefaultSampleDomainTableClient client = new(() => _tableClientMock.Object);

        await client.SetAsync(sample, CancellationToken.None);

        _tableClientMock.Verify(m => m.CreateIfNotExistsAsync(It.IsAny<CancellationToken>()), Times.Once);
        _tableClientMock.Verify(m => m.UpsertEntityAsync(It.IsAny<TableEntitySampleDomain>(), It.IsAny<TableUpdateMode>(),
            It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(RequestFailedException))]
    public async Task DefaultSampleDomainTableClient_SetAsync_WhenAllTableCreationAttemptsFail_ThrowsRequestFailedException()
    {
        int attempts = 5;
        ISampleDomain sample = MockSampleDomain.CreateMock();

        Mock<Response> response = new();

        Exception ex = new RequestFailedException(0, "Mock Message", "TableNotFound", null);

        _tableClientMock.SetupSequence(m => m.UpsertEntityAsync(It.IsAny<TableEntitySampleDomain>(),
                It.IsAny<TableUpdateMode>(), It.IsAny<CancellationToken>()))
            // 6 failures before returning success
            .ThrowsAsync(ex)
            .ThrowsAsync(ex)
            .ThrowsAsync(ex)
            .ThrowsAsync(ex)
            .ThrowsAsync(ex)
            .ThrowsAsync(ex)
            .ReturnsAsync(response.Object);

        DefaultSampleDomainTableClient client = new(() => _tableClientMock.Object, attempts);

        try
        {
            await client.SetAsync(sample, CancellationToken.None);
        }
        finally
        {
            // Should try 5 times to create the table
            _tableClientMock.Verify(m => m.CreateIfNotExistsAsync(It.IsAny<CancellationToken>()),
                Times.Exactly(attempts));

            // Should try 6 times to update (once after the 5th table creation attempt too)
            _tableClientMock.Verify(m => m.UpsertEntityAsync(It.IsAny<TableEntitySampleDomain>(),
                It.IsAny<TableUpdateMode>(), It.IsAny<CancellationToken>()), Times.Exactly(attempts+1));
        }
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(RequestFailedException))]
    public async Task DefaultSampleDomainTableClient_SetAsync_WhenOtherExceptions_ThrowsRequestFailedException()
    {
        ISampleDomain sample = MockSampleDomain.CreateMock();

        Exception ex = new RequestFailedException(0, "Mock Message", "OtherIssue", null);

        _tableClientMock.Setup(m => m.UpsertEntityAsync(It.IsAny<TableEntitySampleDomain>(),
                It.IsAny<TableUpdateMode>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);

        DefaultSampleDomainTableClient client = new(() => _tableClientMock.Object);

        try
        {
            await client.SetAsync(sample, CancellationToken.None);
        }
        finally
        {
            // Should try 5 times to create the table
            _tableClientMock.Verify(m => m.CreateIfNotExistsAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            // Should try 6 times to update (once after the 5th table creation attempt too)
            _tableClientMock.Verify(m => m.UpsertEntityAsync(It.IsAny<TableEntitySampleDomain>(),
                It.IsAny<TableUpdateMode>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

