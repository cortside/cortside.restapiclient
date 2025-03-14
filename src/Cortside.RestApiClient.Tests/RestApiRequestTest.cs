using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using RestSharp;
using Xunit;

namespace Cortside.RestApiClient.Tests {
    public class RestApiRequestTest {
        [Fact]
        public void ShouldConstructWithNoArgs() {
            // act
            var request = new RestApiRequest();

            // assert
            Assert.NotNull(request.RestRequest);
        }

        [Fact]
        public void ShouldConstructWithRestRequest() {
            // arrange
            var rr = new RestRequest();

            // act
            var request = RestApiRequest.From(rr);

            // assert
            Assert.NotNull(request.RestRequest);
            Assert.Equal(rr, request.RestRequest);
        }

        [Fact]
        public void ShouldConstructWithResourceAndMethod() {
            // arrange
            const string url = "https://api.github.com/";

            // act
            var request = new RestApiRequest(url, Method.Options);

            // assert
            Assert.Equal(url, request.Resource);
            Assert.Equal(url, request.RestRequest.Resource);
            Assert.Equal(Method.Options, request.Method);
            Assert.Equal(Method.Options, request.RestRequest.Method);
        }

        [Fact]
        public void ShouldSetAlwaysMultipartFormData() {
            // act
            var request = new RestApiRequest() {
                AlwaysMultipartFormData = true
            };

            // assert
            Assert.True(request.AlwaysMultipartFormData);
            Assert.True(request.RestRequest.AlwaysMultipartFormData);
        }

        [Fact]
        public void ShouldSetMultipartFormQuoteParameters() {
            // act
            var request = new RestApiRequest() {
                MultipartFormQuoteParameters = true
            };

            // assert
            Assert.True(request.MultipartFormQuoteParameters);
            Assert.True(request.RestRequest.MultipartFormQuoteParameters);
        }

        [Fact]
        public void ShouldSetParameters() {
            // act
            var request = new RestApiRequest();

            // assert
            Assert.Empty(request.Parameters);
            Assert.Empty(request.RestRequest.Parameters);

            // act
            request.AddHeader("foo", "bar");

            // assert
            Assert.Contains(request.Parameters, x => x.Name == "foo" && x.Value.ToString() == "bar");
            Assert.Contains(request.RestRequest.Parameters, x => x.Name == "foo" && x.Value.ToString() == "bar");

            // act
            request.RemoveParameter(request.Parameters.First());

            // assert
            Assert.Empty(request.Parameters);
            Assert.Empty(request.RestRequest.Parameters);
        }

        [Fact]
        public void ShouldSetTimeout() {
            // arrange
            const int value = 99;

            // act
            var request = new RestApiRequest() {
                Timeout = TimeSpan.FromMilliseconds(value)
            };

            // assert
            Assert.Equal(value, request.Timeout?.Milliseconds);
            Assert.Equal(value, request.RestRequest.Timeout?.Milliseconds);
        }

        [Fact]
        public void ShouldSetRootElement() {
            // arrange
            const string value = "foo";

            // act
            var request = new RestApiRequest() {
                RootElement = value
            };

            // assert
            Assert.Equal(value, request.RootElement);
            Assert.Equal(value, request.RestRequest.RootElement);
        }

        [Fact]
        public void ShouldSetResource() {
            // arrange
            const string value = "foo";

            // act
            var request = new RestApiRequest() {
                Resource = value
            };

            // assert
            Assert.Equal(value, request.Resource);
            Assert.Equal(value, request.RestRequest.Resource);
        }

        [Fact]
        public void ShouldSetRequestFormat() {
            // arrange
            const DataFormat value = DataFormat.Binary;

            // act
            var request = new RestApiRequest() {
                RequestFormat = value
            };

            // assert
            Assert.Equal(value, request.RequestFormat);
            Assert.Equal(value, request.RestRequest.RequestFormat);
        }

        [Fact]
        public void ShouldSetFormBoundary() {
            // arrange
            const string value = "foo";

            // act
            var request = new RestApiRequest() {
                FormBoundary = value
            };

            // assert
            Assert.Equal(value, request.FormBoundary);
            Assert.Equal(value, request.RestRequest.FormBoundary);
        }

        [Fact]
        public void ShouldAddFile1() {
            // act
            var request = new RestApiRequest();

            // act
            request.AddFile("foo.txt", Array.Empty<byte>(), "foo.txt");

            // assert
            Assert.NotEmpty(request.Files);
            Assert.NotEmpty(request.RestRequest.Files);
        }

        [Fact]
        public void ShouldAddFile2() {
            // arrange
            var request = new RestApiRequest();
            var filename = Path.GetTempFileName();
            File.WriteAllText(filename, "test");
            Assert.True(File.Exists(filename));

            // act
            var file = Path.GetFileName(filename);
            request.AddFile(file, filename);

            // assert
            Assert.NotEmpty(request.Files);
            Assert.NotEmpty(request.RestRequest.Files);
        }

        [Fact]
        public void ShouldAddFile3() {
            // arrange
            var request = new RestApiRequest();
            var filename = Path.GetTempFileName();
            File.WriteAllText(filename, "test");
            Assert.True(File.Exists(filename));

            // act
            var file = Path.GetFileName(filename);
            request.AddFile(file, () => File.OpenRead(filename), filename);

            // assert
            Assert.NotEmpty(request.Files);
            Assert.NotEmpty(request.RestRequest.Files);
        }

        [Fact]
        public void ShouldGetAttempts() {
            // act
            var request = new RestApiRequest();

            // assert
            Assert.Equal(0, request.Attempts);
            Assert.Equal(0, request.RestRequest.Attempts);
        }

        [Fact]
        public void ShouldSetCompletionOption() {
            // arrange
            const HttpCompletionOption value = HttpCompletionOption.ResponseHeadersRead;

            // act
            var request = new RestApiRequest() {
                CompletionOption = value
            };

            // assert
            Assert.Equal(value, request.CompletionOption);
            Assert.Equal(value, request.RestRequest.CompletionOption);
        }
    }
}
