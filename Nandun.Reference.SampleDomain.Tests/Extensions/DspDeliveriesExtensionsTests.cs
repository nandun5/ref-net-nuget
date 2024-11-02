using System.Reflection;
using Nandun.Reference.SampleDomain.Extensions;
using Nandun.Reference.SampleDomain.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nandun.Reference.SampleDomain.Tests.Extensions;

internal class SampleDomainExtensionsTests
{
    [TestMethod]
    [TestCategory("Unit")]
    public void SampleDomainExtensions_MergeTo_WhenInvoked_CopiesAllValuesToTarget()
    {
        ISampleDomain source = MockSampleDomain.CreateMock();
        ISampleDomain target = new MockSampleDomain();

        source.MergeTo(target);

        foreach (PropertyInfo p in source.GetType()
                     .GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            object srcVal = p.GetValue(source);
            object dstVal = p.GetValue(target);

            Assert.AreEqual(srcVal, dstVal);
        }
            
    }
}