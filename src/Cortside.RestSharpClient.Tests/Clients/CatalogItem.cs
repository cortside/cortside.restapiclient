using System;

namespace Cortside.RestSharpClient.Tests.Mocks {
    internal class CatalogItem {
        public CatalogItem() {
        }

        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal UnitPrice { get; set; }
    }
}