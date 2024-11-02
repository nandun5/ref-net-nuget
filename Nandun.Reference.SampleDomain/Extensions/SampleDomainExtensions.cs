namespace Nandun.Reference.SampleDomain.Extensions;

/// <summary>
/// Some methods to make life easy when dealing with <see cref="ISampleDomain"/>
/// </summary>
public static class SampleDomainExtensions
{
    /// <summary>
    /// Merges the values of <see>
    ///     <cref>source</cref>
    /// </see>
    /// to <see>
    ///     <cref>target</cref>
    /// </see>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public static void MergeTo(this ISampleDomain source, ISampleDomain target)
    {
        target.Id = source.Id;
        target.ProcessingDate = source.ProcessingDate;
        target.SampleCode = source.SampleCode;
    }
}