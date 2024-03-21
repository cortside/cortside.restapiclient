using System;
using System.Globalization;
using System.Threading.Tasks;
using Cortside.Common.Testing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using RestSharp;
using Xunit;

namespace Cortside.RestApiClient.Tests {
    public class DateTimeHandlingTest {
        [Fact]
        public void SerializeDateTime() {
            using (new ScopedLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"))) {
                var serializer = new JsonNetSerializer();
                var json = serializer.Serialize(new DateTime(2013, 1, 21, 1, 2, 3, DateTimeKind.Local));
                json = json.Substring(1, json.Length - 2);
                Assert.Equal("2013-01-21T08:02:03Z", json);
            }
        }

        [Fact]
        public void DeserializeIso8601UtcDateTime() {
            using (new ScopedLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"))) {
                // act
                var date = DateTime.Parse("2013-01-21T08:02:03Z", CultureInfo.InvariantCulture);

                // assert
                Assert.Equal(new DateTime(2013, 1, 21, 1, 2, 3, DateTimeKind.Local), date);
            }
        }

        [Fact]
        public void DeserializeIso8601OffsetDateTime() {
            using (new ScopedLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"))) {
                // act
                var date = DateTime.Parse("2013-01-21T08:02:03-05:00", CultureInfo.InvariantCulture);

                // assert
                Assert.Equal(new DateTime(2013, 1, 21, 6, 2, 3, DateTimeKind.Local), date);
            }
        }

        [Fact]
        public void SerializeInt() {
            var serializer = new JsonNetSerializer();
            var json = serializer.Serialize(1);

            Assert.Equal("1", json);
        }

        [Fact]
        public async Task ShouldSerializeDateTimeInQueryString() {
            using (new ScopedLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"))) {
                var options = new RestApiClientOptions("https://postman-echo.com");
                var client = new RestApiClient(NullLogger.Instance, new HttpContextAccessor(), options);

                var request = new RestApiRequest("get", Method.Get);
                request.AddQueryParameter("foo1", "bar1");
                request.AddQueryParameter("foo2", "bar2");
                request.AddParameter("date", new DateTime(2013, 1, 21, 1, 2, 3, DateTimeKind.Local));
                var response = await client.GetAsync(request);

                Assert.NotNull(response);
                Assert.Equal("2013-01-21T08:02:03Z", request.Parameters.TryFind("date").Value);
                Assert.Equal("https://postman-echo.com/get?foo1=bar1&foo2=bar2&date=2013-01-21T08%3a02%3a03Z",
                    client.BuildUri(request).ToString());
            }
        }
    }
}
