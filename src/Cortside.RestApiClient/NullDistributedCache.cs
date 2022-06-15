using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Cortside.RestApiClient {
    internal class NullDistributedCache : IDistributedCache {
        public byte[] Get(string key) {
            throw new System.NotSupportedException();
        }

        public Task<byte[]> GetAsync(string key, CancellationToken token = default) {
            return Task.FromResult<byte[]>(null);
        }

        public void Refresh(string key) {
            throw new System.NotSupportedException();
        }

        public Task RefreshAsync(string key, CancellationToken token = default) {
            return Task.CompletedTask;
        }

        public void Remove(string key) {
            throw new System.NotSupportedException();
        }

        public Task RemoveAsync(string key, CancellationToken token = default) {
            return Task.CompletedTask;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options) {
            throw new System.NotSupportedException();
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default) {
            return Task.CompletedTask;
        }
    }
}
