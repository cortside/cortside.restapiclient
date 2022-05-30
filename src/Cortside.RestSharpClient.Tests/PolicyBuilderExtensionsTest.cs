using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Polly;
using RestSharp;
using Xunit;

namespace Cortside.RestSharpClient.Tests {
    public class PolicyBuilderExtensionsTest {
        [Fact]
        public void Should_be_able_to_reference_HandleTransientHttpError() {
            PolicyBuilderExtensions.HandleTransientHttpError()
                .Should().BeOfType<PolicyBuilder<RestResponse>>();
        }

        [Fact]
        public void HandleTransientHttpError_should_handle_HttpRequestException() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = PolicyBuilderExtensions.HandleTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => throw new HttpRequestException());

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void HandleTransientHttpError_should_handle_HttpStatusCode_RequestTimeout() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = PolicyBuilderExtensions.HandleTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.RequestTimeout)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void HandleTransientHttpError_should_handle_HttpStatusCode_InternalServerError() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = PolicyBuilderExtensions.HandleTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.InternalServerError)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void HandleTransientHttpError_should_not_handle_HttpStatusCode_BadRequest() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = PolicyBuilderExtensions.HandleTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.BadRequest)));

            policyHandled.Should().BeFalse();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpError() {
            Policy.Handle<CustomException>().OrTransientHttpError()
                .Should().BeOfType<PolicyBuilder<RestResponse>>();
        }

        [Fact]
        public void OrTransientHttpError_should_handle_HttpRequestException() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => throw new HttpRequestException());

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpError_should_handle_HttpStatusCode_RequestTimeout() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.RequestTimeout)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpError_should_handle_HttpStatusCode_InternalServerError() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.InternalServerError)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpError_should_not_handle_HttpStatusCode_BadRequest() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.BadRequest)));

            policyHandled.Should().BeFalse();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpError_onGenericPolicyBuilder() {
            Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .Should().BeOfType<PolicyBuilder<RestResponse>>();
        }

        [Fact]
        public void OrTransientHttpError_onGenericPolicyBuilder_should_handle_HttpRequestException() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => throw new HttpRequestException());

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpError_onGenericPolicyBuilder_should_handle_HttpStatusCode_RequestTimeout() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.RequestTimeout)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpError_onGenericPolicyBuilder_should_handle_HttpStatusCode_InternalServerError() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.InternalServerError)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpError_onGenericPolicyBuilder_should_not_handle_HttpStatusCode_BadRequest() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(token => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.BadRequest)));

            policyHandled.Should().BeFalse();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpStatusCode() {
            Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .Should().BeOfType<PolicyBuilder<RestResponse>>();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpStatusCode_onGenericPolicyBuilder() {
            Policy<RestResponse>.Handle<CustomException>().OrTransientHttpStatusCode()
                .Should().BeOfType<PolicyBuilder<RestResponse>>();
        }

        [Fact]
        public void OrTransientHttpStatusCode_should_not_handle_HttpRequestException() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(token => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => throw new HttpRequestException());

            policyHandled.Should().BeFalse();
        }

        [Fact]
        public void OrTransientHttpStatusCode_should_handle_HttpStatusCode_RequestTimeout() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(token => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.RequestTimeout)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpStatusCode_should_handle_HttpStatusCode_InternalServerError() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(token => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.InternalServerError)));

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public void OrTransientHttpStatusCode_should_not_handle_HttpStatusCode_BadRequest() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(token => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.BadRequest)));

            policyHandled.Should().BeFalse();
        }
    }
}
