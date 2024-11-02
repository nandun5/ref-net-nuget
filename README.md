# Introduction 
This solution is to provide  sample Table Storage Support Sample Domain.

# How to use me

## Add Nuget to Project

This nuget is maintained in the [Nuget Repository](https://github.com/nandun5/ref-net-nuget).

Search for `Nandun.Reference.SampleDomain` package in NuGet package manager 

or run the following command

`Install-Package Nandun.Reference.SampleDomain`



## Dependency Injection

### Configure DI
First, configure dependency injection in start-up code of the project

```cs

services.AddTableStorageSampleDomain("DefaultEndpointsProtocol=https;AccountName=nandunstorage;AccountKey=__REPLACE_KEY__;BlobEndpoint=https://nandunstorage.blob.core.windows.net/;QueueEndpoint=https://nandunstorage.queue.core.windows.net/;TableEndpoint=https://nandunstorage.table.core.windows.net/;FileEndpoint=https://nandunstorage.file.core.windows.net/;")

```

### Inject to calling class

Simply add a reference to `ISampleDomainTableClient` in to the constructor of the calling class.

```cs
private readonly ISampleDomainTableClient _SampleDomainClient

public MyClass(ISampleDomainTableClient SampleDomainClient){
    _SampleDomainClient = SampleDomainClient;
}

```
