[![Build status](https://ci.appveyor.com/api/projects/status/uin64c4fqg946mou?svg=true)](https://ci.appveyor.com/project/cortside/cortside-restapiclient)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=cortside_cortside.restapiclient&metric=alert_status)](https://sonarcloud.io/dashboard?id=cortside_cortside.restapiclient)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=cortside_cortside.restapiclient&metric=coverage)](https://sonarcloud.io/dashboard?id=cortside_cortside.restapiclient)

## Cortside.RestApiClient

Fully featured REST client for .Net using RestSharp

Support [synchronous communication](https://github.com/cortside/guidelines/blob/master/docs/architecture/Microservices.md#synchronous-communication) between services handled by use of HTTP requests and responses responses.  The [base client](src/cortside.restapiclient/RestApiClient.cs) by making use of [RestSharp](https://github.com/restsharp/RestSharp).  The base client itself handles:

* authentication
* logging of operations
* correlation
* serialization
* caching
* error handling

Some inspiration taken from this [article](https://exceptionnotfound.net/building-the-ultimate-restsharp-client-in-asp-net-and-csharp/).
