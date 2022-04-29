[![Build status](https://ci.appveyor.com/api/projects/status/43l1ckgn806lqxjx?svg=true)](https://ci.appveyor.com/project/cortside/cortside-domainevent)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=cortside_cortside.common&metric=alert_status)](https://sonarcloud.io/dashboard?id=cortside_cortside.domainevent)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=cortside_cortside.domainevent&metric=coverage)](https://sonarcloud.io/dashboard?id=cortside_cortside.domainevent)

## Cortside.RestSharpClient

Fully featured REST client for .Net using RestSharp

Support [synchronous communication](https://github.com/cortside/guidelines/blob/master/docs/architecture/Microservices.md#synchronous-communication) between services handled by use of HTTP requests and responses responses.  The [base client](src/cortside.restsharpclient/RestSharpClient.cs) by making use of [RestSharp](https://github.com/restsharp/RestSharp).  The base client itself handles:

* authentication
* logging of operations
* correlation
* serialization
* caching
* error handling

Some inspiration taken from this [article](https://exceptionnotfound.net/building-the-ultimate-restsharp-client-in-asp-net-and-csharp/).
