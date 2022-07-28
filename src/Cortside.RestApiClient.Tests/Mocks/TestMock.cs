using System;
using Cortside.MockServer;
using Cortside.RestApiClient.Tests.Clients.CatalogApi;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Cortside.RestApiClient.Tests.Mocks {
    public class TestMock : IMockHttpServerBuilder {
        public void Configure(WireMockServer server) {
            var rnd = new Random();

            server
                .Given(
                    Request.Create().WithPath("/api/v1/items/search")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(303)
                        .WithHeader("Content-Type", "application/json")
                        .WithHeader("Location", "/api/v1/items/search")
                );

            server
                .Given(
                    Request.Create().WithPath("/api/v1/items/search")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(r => JsonConvert.SerializeObject(new CatalogItem() {
                            ItemId = Guid.NewGuid(),
                            Name = $"Item {r.PathSegments[3]}",
                            Sku = r.PathSegments[3],
                            UnitPrice = new decimal(rnd.Next(10000) / 100.0)
                        }))
                );

            server
                .Given(
                    Request.Create().WithPath("/api/v1/items")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithHeader("Location", "/api/v1/items/1234")
                        .WithBody(r => JsonConvert.SerializeObject(new CatalogItem() {
                            ItemId = Guid.NewGuid(),
                            Name = "Item 1234",
                            Sku = "1234",
                            UnitPrice = 15.99M
                        }))
                );

            server
                .Given(
                    Request.Create().WithPath("/api/v1/items/*")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(r => JsonConvert.SerializeObject(new CatalogItem() {
                            ItemId = Guid.NewGuid(),
                            Name = $"Item {r.PathSegments[3]}",
                            Sku = r.PathSegments[3],
                            UnitPrice = new decimal(rnd.Next(10000) / 100.0)
                        }))
                );

            server
                .Given(
                    Request.Create().WithPath("/api/v1/timeout")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(@"{ ""msg"": ""Hello I'm a little bit slow!"" }")
                        .WithDelay(TimeSpan.FromSeconds(10))
                );
        }
    }
}
