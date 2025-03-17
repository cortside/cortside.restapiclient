using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Cortside.RestApiClient.Tests.Exceptions;
using Shouldly;
using Polly;
using RestSharp;
using Xunit;

namespace Cortside.RestApiClient.Tests {
    public class PolicyBuilderExtensionsTest {
        [Fact]
        public void Should_be_able_to_reference_HandleTransientHttpError() {
            PolicyBuilderExtensions.HandleTransientHttpError()
                .ShouldBeOfType<PolicyBuilder<RestResponse>>();
        }

        [Fact]
        public async Task HandleTransientHttpError_should_handle_HttpRequestExceptionAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = PolicyBuilderExtensions.HandleTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => throw new HttpRequestException());

            policyHandled.ShouldBeTrue();
        }

        [Fact]
        public async Task HandleTransientHttpError_should_handle_HttpStatusCode_RequestTimeoutAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = PolicyBuilderExtensions.HandleTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.RequestTimeout)));

            policyHandled.ShouldBeTrue();
        }

        [Fact]
        public async Task HandleTransientHttpError_should_handle_HttpStatusCode_InternalServerErrorAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = PolicyBuilderExtensions.HandleTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.InternalServerError)));

            policyHandled.ShouldBeTrue();
        }

        [Fact]
        public async Task HandleTransientHttpError_should_not_handle_HttpStatusCode_BadRequestAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = PolicyBuilderExtensions.HandleTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.BadRequest)));

            policyHandled.ShouldBeFalse();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpError() {
            Policy.Handle<CustomException>().OrTransientHttpError()
                .ShouldBeOfType<PolicyBuilder<RestResponse>>();
        }

        [Fact]
        public async Task OrTransientHttpError_should_handle_HttpRequestExceptionAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => throw new HttpRequestException());

            policyHandled.ShouldBeTrue();
        }

        [Fact]
        public async Task OrTransientHttpError_should_handle_HttpStatusCode_RequestTimeoutAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.RequestTimeout)));

            policyHandled.ShouldBeTrue();
        }

        [Fact]
        public async Task OrTransientHttpError_should_handle_HttpStatusCode_InternalServerErrorAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.InternalServerError)));

            policyHandled.ShouldBeTrue();
        }

        [Fact]
        public async Task OrTransientHttpError_should_not_handle_HttpStatusCode_BadRequestAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.BadRequest)));

            policyHandled.ShouldBeFalse();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpError_onGenericPolicyBuilder() {
            Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .ShouldBeOfType<PolicyBuilder<RestResponse>>();
        }

        [Fact]
        public async Task OrTransientHttpError_onGenericPolicyBuilder_should_handle_HttpRequestExceptionAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => throw new HttpRequestException());

            policyHandled.ShouldBeTrue();
        }

        [Fact]
        public async Task OrTransientHttpError_onGenericPolicyBuilder_should_handle_HttpStatusCode_RequestTimeoutAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.RequestTimeout)));

            policyHandled.ShouldBeTrue();
        }

        [Fact]
        public async Task OrTransientHttpError_onGenericPolicyBuilder_should_handle_HttpStatusCode_InternalServerErrorAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.InternalServerError)));

            policyHandled.ShouldBeTrue();
        }

        [Fact]
        public async Task OrTransientHttpError_onGenericPolicyBuilder_should_not_handle_HttpStatusCode_BadRequestAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy<RestResponse>.Handle<CustomException>().OrTransientHttpError()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.BadRequest)));

            policyHandled.ShouldBeFalse();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpStatusCode() {
            Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .ShouldBeOfType<PolicyBuilder<RestResponse>>();
        }

        [Fact]
        public void Should_be_able_to_reference_OrTransientHttpStatusCode_onGenericPolicyBuilder() {
            Policy<RestResponse>.Handle<CustomException>().OrTransientHttpStatusCode()
                .ShouldBeOfType<PolicyBuilder<RestResponse>>();
        }

        [Fact]
        public async Task OrTransientHttpStatusCode_should_not_handle_HttpRequestExceptionAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => throw new CustomException());

            policyHandled.ShouldBeTrue();
        }

        [Fact]
        public async Task OrTransientHttpStatusCode_should_handle_HttpStatusCode_RequestTimeoutAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.RequestTimeout)));

            policyHandled.ShouldBeTrue();
        }

        [Fact]
        public async Task OrTransientHttpStatusCode_should_handle_HttpStatusCode_InternalServerErrorAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.InternalServerError)));

            policyHandled.ShouldBeTrue();
        }

        [Fact]
        public async Task OrTransientHttpStatusCode_should_not_handle_HttpStatusCode_BadRequestAsync() {
            var policyHandled = false;
            IAsyncPolicy<RestResponse> policy = Policy.Handle<CustomException>().OrTransientHttpStatusCode()
                .FallbackAsync(_ => {
                    policyHandled = true;
                    return Task.FromResult<RestResponse>(null);
                });

            await policy.ExecuteAsync(() => Task.FromResult(RestResponseExtensions.FromStatusCode(HttpStatusCode.BadRequest)));

            policyHandled.ShouldBeFalse();
        }
    }
}
