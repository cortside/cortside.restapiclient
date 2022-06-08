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
        public async Task HandleTransientHttpError_should_handle_HttpRequestExceptionAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = PolicyBuilderExtensions.HandleTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => throw new HttpRequestException()).ConfigureAwait(false);

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public async Task HandleTransientHttpError_should_handle_HttpStatusCode_RequestTimeoutAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = PolicyBuilderExtensions.HandleTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.RequestTimeout))).ConfigureAwait(false);

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public async Task HandleTransientHttpError_should_handle_HttpStatusCode_InternalServerErrorAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = PolicyBuilderExtensions.HandleTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.InternalServerError))).ConfigureAwait(false);

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public async Task HandleTransientHttpError_should_not_handle_HttpStatusCode_BadRequestAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = PolicyBuilderExtensions.HandleTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.BadRequest))).ConfigureAwait(false);

            policyHandled.Should().BeFalse();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpError() {
            Policy.Handle<CustomException>().OrTransientHttpError()
                .Should().BeOfType<PolicyBuilder<RestResponse>>();
        }

        [Fact]
        public async Task OrTransientHttpError_should_handle_HttpRequestExceptionAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => throw new HttpRequestException()).ConfigureAwait(false);

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public async Task OrTransientHttpError_should_handle_HttpStatusCode_RequestTimeoutAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.RequestTimeout))).ConfigureAwait(false);

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public async Task OrTransientHttpError_should_handle_HttpStatusCode_InternalServerErrorAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.InternalServerError))).ConfigureAwait(false);

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public async Task OrTransientHttpError_should_not_handle_HttpStatusCode_BadRequestAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.BadRequest))).ConfigureAwait(false);

            policyHandled.Should().BeFalse();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpError_onGenericPolicyBuilder() {
            Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .Should().BeOfType<PolicyBuilder<RestResponse>>();
        }

        [Fact]
        public async Task OrTransientHttpError_onGenericPolicyBuilder_should_handle_HttpRequestExceptionAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => throw new HttpRequestException()).ConfigureAwait(false);

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public async Task OrTransientHttpError_onGenericPolicyBuilder_should_handle_HttpStatusCode_RequestTimeoutAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.RequestTimeout))).ConfigureAwait(false);

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public async Task OrTransientHttpError_onGenericPolicyBuilder_should_handle_HttpStatusCode_InternalServerErrorAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.InternalServerError))).ConfigureAwait(false);

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public async Task OrTransientHttpError_onGenericPolicyBuilder_should_not_handle_HttpStatusCode_BadRequestAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.BadRequest))).ConfigureAwait(false);

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
        public async Task OrTransientHttpStatusCode_should_not_handle_HttpRequestExceptionAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => throw new CustomException()).ConfigureAwait(false);

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public async Task OrTransientHttpStatusCode_should_handle_HttpStatusCode_RequestTimeoutAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.RequestTimeout))).ConfigureAwait(false);

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public async Task OrTransientHttpStatusCode_should_handle_HttpStatusCode_InternalServerErrorAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.InternalServerError))).ConfigureAwait(false);

            policyHandled.Should().BeTrue();
        }

        [Fact]
        public async Task OrTransientHttpStatusCode_should_not_handle_HttpStatusCode_BadRequestAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.BadRequest))).ConfigureAwait(false);

            policyHandled.Should().BeFalse();
        }
    }
}
