#pragma warning disable RCS1224 // Make method an extension method.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Cortside.Common.Validation;
using Polly;
using Polly.Contrib.WaitAndRetry;
using RestSharp;

namespace Cortside.RestSharpClient {
    /// <summary>
    /// Contains convenience methods for configuring policies to handle conditions typically representing transient faults when making <see cref="RestClient"/> requests.
    /// </summary>
    public static class PolicyBuilderExtensions {
        private static readonly Func<RestResponse, bool> TransientHttpStatusCodePredicate = (response) => {
            return (int)response.StatusCode >= 500 || response.StatusCode == HttpStatusCode.RequestTimeout;
        };

        public static IEnumerable<TimeSpan> Jitter(int firstRetrySeconds, int retryCount) {
            return Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(firstRetrySeconds), retryCount: retryCount);
        }

        public static PolicyBuilder<RestResponse> Handle<T>() where T : Exception {
            return Policy<RestResponse>.Handle<T>();
        }

        /// <summary>
        /// Builds a <see cref="PolicyBuilder{RestResponse}"/> to configure a <see cref="Policy{RestResponse}"/> which will handle <see cref="RestClient"/> requests that fail with conditions indicating a transient failure.
        /// <para>The conditions configured to be handled are:
        /// <list type="bullet">
        /// <item><description>Network failures (as <see cref="HttpRequestException"/>)</description></item>
        /// <item><description>HTTP 5XX status codes (server errors)</description></item>
        /// <item><description>HTTP 408 status code (request timeout)</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>The <see cref="PolicyBuilder{RestResponse}"/> pre-configured to handle <see cref="RestClient"/> requests that fail with conditions indicating a transient failure. </returns>
        public static PolicyBuilder<RestResponse> HandleTransientHttpError() {
            return Policy<RestResponse>.Handle<HttpRequestException>().OrTransientHttpStatusCode();
        }

        /// <summary>
        /// Configures the <see cref="PolicyBuilder{RestResponse}"/> to handle <see cref="RestClient"/> requests that fail with <see cref="HttpStatusCode"/>s indicating a transient failure.
        /// <para>The <see cref="HttpStatusCode"/>s configured to be handled are:
        /// <list type="bullet">
        /// <item><description>HTTP 5XX status codes (server errors)</description></item>
        /// <item><description>HTTP 408 status code (request timeout)</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>The <see cref="PolicyBuilder{RestResponse}"/> pre-configured to handle <see cref="RestClient"/> requests that fail with <see cref="HttpStatusCode"/>s indicating a transient failure. </returns>
        public static PolicyBuilder<RestResponse> OrTransientHttpStatusCode(this PolicyBuilder policyBuilder) {
            Guard.From.Null(policyBuilder, nameof(policyBuilder));

            return policyBuilder.OrResult(TransientHttpStatusCodePredicate);
        }

        /// <summary>
        /// Configures the <see cref="PolicyBuilder{RestResponse}"/> to handle <see cref="RestClient"/> requests that fail with <see cref="HttpStatusCode"/>s indicating a transient failure.
        /// <para>The <see cref="HttpStatusCode"/>s configured to be handled are:
        /// <list type="bullet">
        /// <item><description>HTTP 5XX status codes (server errors)</description></item>
        /// <item><description>HTTP 408 status code (request timeout)</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>The <see cref="PolicyBuilder{RestResponse}"/> pre-configured to handle <see cref="RestClient"/> requests that fail with <see cref="HttpStatusCode"/>s indicating a transient failure. </returns>
        public static PolicyBuilder<RestResponse> OrTransientHttpStatusCode(this PolicyBuilder<RestResponse> policyBuilder) {
            Guard.From.Null(policyBuilder, nameof(policyBuilder));

            return policyBuilder.OrResult(TransientHttpStatusCodePredicate);
        }

        /// <summary>
        /// Configures the <see cref="PolicyBuilder{RestResponse}"/> to handle <see cref="RestClient"/> requests that fail with conditions indicating a transient failure.
        /// <para>The conditions configured to be handled are:
        /// <list type="bullet">
        /// <item><description>Network failures (as <see cref="HttpRequestException"/>)</description></item>
        /// <item><description>HTTP 5XX status codes (server errors)</description></item>
        /// <item><description>HTTP 408 status code (request timeout)</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>The <see cref="PolicyBuilder{RestResponse}"/> pre-configured to handle <see cref="RestClient"/> requests that fail with conditions indicating a transient failure. </returns>
        public static PolicyBuilder<RestResponse> OrTransientHttpError(this PolicyBuilder policyBuilder) {
            Guard.From.Null(policyBuilder, nameof(policyBuilder));

            return policyBuilder.Or<HttpRequestException>().OrTransientHttpStatusCode();
        }

        /// <summary>
        /// Configures the <see cref="PolicyBuilder{RestResponse}"/> to handle <see cref="RestClient"/> requests that fail with conditions indicating a transient failure.
        /// <para>The conditions configured to be handled are:
        /// <list type="bullet">
        /// <item><description>Network failures (as <see cref="HttpRequestException"/>)</description></item>
        /// <item><description>HTTP 5XX status codes (server errors)</description></item>
        /// <item><description>HTTP 408 status code (request timeout)</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>The <see cref="PolicyBuilder{RestResponse}"/> pre-configured to handle <see cref="RestClient"/> requests that fail with conditions indicating a transient failure. </returns>
        public static PolicyBuilder<RestResponse> OrTransientHttpError(this PolicyBuilder<RestResponse> policyBuilder) {
            Guard.From.Null(policyBuilder, nameof(policyBuilder));

            return policyBuilder.Or<HttpRequestException>().OrTransientHttpStatusCode();
        }
    }
}
