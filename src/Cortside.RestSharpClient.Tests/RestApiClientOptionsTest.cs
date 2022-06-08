using System;
using System.Net;
using RestSharp;
using Xunit;

namespace Cortside.RestSharpClient.Tests {
    public class RestApiClientOptionsTest {
        [Fact]
        public void ShouldSetupNoArgsDefaults() {
            // act
            RestApiClientOptions options = new RestApiClientOptions();

            // assert
            Assert.NotNull(options.Serializer);
            Assert.NotNull(options.Policy);
            Assert.NotNull(options.Cache);
            Assert.NotNull(options.Options);
        }

        [Fact]
        public void ShouldSetupDefaultsWithUrl() {
            // arrange
            var url = "https://api.github.com/";

            // act
            RestApiClientOptions options = new RestApiClientOptions(url);

            // assert
            Assert.NotNull(options.Serializer);
            Assert.NotNull(options.Policy);
            Assert.NotNull(options.Cache);
            Assert.NotNull(options.Options);

            Assert.Equal(url, options.BaseUrl.ToString());
            Assert.Equal(url, options.Options.BaseUrl.ToString());
        }

        [Fact]
        public void ShouldSetupDefaultsWithUri() {
            // arrange
            var url = "https://api.github.com/";

            // act
            RestApiClientOptions options = new RestApiClientOptions(new Uri(url));

            // assert
            Assert.NotNull(options.Serializer);
            Assert.NotNull(options.Policy);
            Assert.NotNull(options.Cache);
            Assert.NotNull(options.Options);

            Assert.Equal(url, options.BaseUrl.ToString());
            Assert.Equal(url, options.Options.BaseUrl.ToString());
        }

        [Fact]
        public void ShouldCreateFromOptions() {
            // arrange
            var url = "https://api.github.com/";

            // act
            RestApiClientOptions options = RestApiClientOptions.From(new RestClientOptions(url));

            // assert
            Assert.NotNull(options.Serializer);
            Assert.NotNull(options.Policy);
            Assert.NotNull(options.Cache);
            Assert.NotNull(options.Options);

            Assert.Equal(url, options.BaseUrl.ToString());
            Assert.Equal(url, options.Options.BaseUrl.ToString());
        }

        [Fact]
        public void ShouldSetBaseUrl() {
            // arrange
            var url = "https://api.github.com/";

            // act
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(url)
            };

            // assert
            Assert.Equal(url, options.BaseUrl.ToString());
            Assert.Equal(url, options.Options.BaseUrl.ToString());
        }

        [Fact]
        public void ShouldSetUseDefaultCredentials() {
            // arrange
            RestApiClientOptions options = new RestApiClientOptions();

            // act
            options.UseDefaultCredentials = true;

            // assert
            Assert.True(options.UseDefaultCredentials);
            Assert.True(options.Options.UseDefaultCredentials);
        }

        [Fact]
        public void ShouldSetDisableCharset() {
            // arrange
            RestApiClientOptions options = new RestApiClientOptions();

            // act
            options.DisableCharset = true;

            // assert
            Assert.True(options.DisableCharset);
            Assert.True(options.Options.DisableCharset);
        }

        [Fact]
        public void ShouldSetAutomaticDecompression() {
            // arrange
            RestApiClientOptions options = new RestApiClientOptions();

            // act
            options.AutomaticDecompression = DecompressionMethods.Deflate;

            // assert
            Assert.Equal(DecompressionMethods.Deflate, options.AutomaticDecompression);
            Assert.Equal(DecompressionMethods.Deflate, options.Options.AutomaticDecompression);
        }

        [Fact]
        public void ShouldSetMaxRedirects() {
            // arrange
            RestApiClientOptions options = new RestApiClientOptions();

            // act
            options.MaxRedirects = 99;

            // assert
            Assert.Equal(99, options.MaxRedirects);
            Assert.Equal(99, options.Options.MaxRedirects);
        }

    }
}
