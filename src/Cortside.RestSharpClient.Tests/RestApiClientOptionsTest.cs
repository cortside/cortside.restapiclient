using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Moq;
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
            const string url = "https://api.github.com/";

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
            const string url = "https://api.github.com/";

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
            const string url = "https://api.github.com/";

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
            const string url = "https://api.github.com/";

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
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                UseDefaultCredentials = true
            };

            // assert
            Assert.True(options.UseDefaultCredentials);
            Assert.True(options.Options.UseDefaultCredentials);
        }

        [Fact]
        public void ShouldSetDisableCharset() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                DisableCharset = true
            };

            // assert
            Assert.True(options.DisableCharset);
            Assert.True(options.Options.DisableCharset);
        }

        [Fact]
        public void ShouldSetAutomaticDecompression() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                AutomaticDecompression = DecompressionMethods.Deflate
            };

            // assert
            Assert.Equal(DecompressionMethods.Deflate, options.AutomaticDecompression);
            Assert.Equal(DecompressionMethods.Deflate, options.Options.AutomaticDecompression);
        }

        [Fact]
        public void ShouldSetMaxRedirects() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                MaxRedirects = 99
            };

            // assert
            Assert.Equal(99, options.MaxRedirects);
            Assert.Equal(99, options.Options.MaxRedirects);
        }

        [Fact]
        public void ShouldSetUserAgent() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                UserAgent = "foo"
            };

            // assert
            Assert.Equal("foo", options.UserAgent);
            Assert.Equal("foo", options.Options.UserAgent);
        }

        [Fact]
        public void ShouldSetMaxTimeout() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                MaxTimeout = 99
            };

            // assert
            Assert.Equal(99, options.MaxTimeout);
            Assert.Equal(99, options.Options.MaxTimeout);
        }

        [Fact]
        public void ShouldSetPreAuthenticate() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                PreAuthenticate = true
            };

            // assert
            Assert.True(options.PreAuthenticate);
            Assert.True(options.Options.PreAuthenticate);
        }

        [Fact]
        public void ShouldSetThrowOnDeserializationError() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                ThrowOnDeserializationError = true
            };

            // assert
            Assert.True(options.ThrowOnDeserializationError);
            Assert.False(options.Options.ThrowOnDeserializationError);
        }

        [Fact]
        public void ShouldSetFailOnDeserializationError() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                FailOnDeserializationError = true
            };

            // assert
            Assert.True(options.FailOnDeserializationError);
            Assert.True(options.Options.FailOnDeserializationError);
        }

        [Fact]
        public void ShouldSetThrowOnAnyError() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                ThrowOnAnyError = true
            };

            // assert
            Assert.True(options.ThrowOnAnyError);
            Assert.False(options.Options.ThrowOnAnyError);
        }

        [Fact]
        public void ShouldSetAllowMultipleDefaultParametersWithSameName() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                AllowMultipleDefaultParametersWithSameName = true
            };

            // assert
            Assert.True(options.AllowMultipleDefaultParametersWithSameName);
            Assert.True(options.Options.AllowMultipleDefaultParametersWithSameName);
        }

        [Fact]
        public void ShouldSetFollowRedirects() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                FollowRedirects = false
            };

            // assert
            Assert.False(options.FollowRedirects);

            // underlying restsharpoptions should be false as this is manually handled
            options.FollowRedirects = true;
            Assert.False(options.Options.FollowRedirects);
        }

        [Fact]
        public void ShouldSetBaseHost() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                BaseHost = "foo"
            };

            // assert
            Assert.Equal("foo", options.BaseHost);
            Assert.Equal("foo", options.Options.BaseHost);
        }

        [Fact]
        public void ShouldSetRemoteCertificateValidationCallback() {
            // arrange
            RestApiClientOptions options = new RestApiClientOptions();
            var o = new Mock<RemoteCertificateValidationCallback>().Object;

            // act
            options.RemoteCertificateValidationCallback = o;

            // assert
            Assert.Equal(o, options.RemoteCertificateValidationCallback);
            Assert.Equal(o, options.Options.RemoteCertificateValidationCallback);
        }

        [Fact]
        public void ShouldSetEncoding() {
            // act
            RestApiClientOptions options = new RestApiClientOptions {
                Encoding = Encoding.BigEndianUnicode
            };

            // assert
            Assert.Equal(Encoding.BigEndianUnicode, options.Encoding);
            Assert.Equal(Encoding.BigEndianUnicode, options.Options.Encoding);
        }

        [Fact]
        public void ShouldSetCookieContainer() {
            // arrange
            RestApiClientOptions options = new RestApiClientOptions();
            var o = new Mock<CookieContainer>().Object;

            // act
            options.CookieContainer = o;

            // assert
            Assert.Equal(o, options.CookieContainer);
            Assert.Equal(o, options.Options.CookieContainer);
        }

        [Fact]
        public void ShouldSetCachePolicy() {
            // arrange
            RestApiClientOptions options = new RestApiClientOptions();
            var o = new Mock<CacheControlHeaderValue>().Object;

            // act
            options.CachePolicy = o;

            // assert
            Assert.Equal(o, options.CachePolicy);
            Assert.Equal(o, options.Options.CachePolicy);
        }

        [Fact]
        public void ShouldSetProxy() {
            // arrange
            RestApiClientOptions options = new RestApiClientOptions();
            var o = new Mock<IWebProxy>().Object;

            // act
            options.Proxy = o;

            // assert
            Assert.Equal(o, options.Proxy);
            Assert.Equal(o, options.Options.Proxy);
        }

        [Fact]
        public void ShouldSetClientCertificates() {
            // arrange
            RestApiClientOptions options = new RestApiClientOptions();
            var o = new Mock<X509CertificateCollection>().Object;

            // act
            options.ClientCertificates = o;

            // assert
            Assert.Equal(o, options.ClientCertificates);
            Assert.Equal(o, options.Options.ClientCertificates);
        }

        [Fact]
        public void ShouldSetCredentials() {
            // arrange
            RestApiClientOptions options = new RestApiClientOptions();
            var o = new Mock<ICredentials>().Object;

            // act
            options.Credentials = o;

            // assert
            Assert.Equal(o, options.Credentials);
            Assert.Equal(o, options.Options.Credentials);
        }

        [Fact]
        public void ShouldSetConfigureMessageHandler() {
            // arrange
            RestApiClientOptions options = new RestApiClientOptions();
            var o = new Mock<HttpMessageHandler>().Object;

            // act
            options.ConfigureMessageHandler = _ => o;

            // assert
            Assert.NotNull(options.ConfigureMessageHandler);
            Assert.NotNull(options.Options.ConfigureMessageHandler);
        }
    }
}
