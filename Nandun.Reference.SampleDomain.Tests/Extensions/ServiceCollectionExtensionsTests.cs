using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nandun.Reference.SampleDomain.Tests.Extensions
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void
            ServiceCollectionExtensions_AddTableStorageSampleDomain_WhenCalled_ReturnsServiceCollection()
        {
            ServiceCollection services = new();

            Assert.AreSame(services, services.AddTableStorageSampleDomain("test-connection-string"));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void
            ServiceCollectionExtensions_AddTableStorageSampleDomain_WhenCalled_ConfiguresDependencies()
        {
            ServiceCollection services = new();

            services.AddTableStorageSampleDomain("test-connection-string");

            ISampleDomainTableClient client = services.BuildServiceProvider().GetService<ISampleDomainTableClient>();

            Assert.IsNotNull(client);
        }
    }
}
