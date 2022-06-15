using System;

namespace Cortside.RestApiClient.Tests.Clients.CatalogApi {
    public class CatalogItem {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
