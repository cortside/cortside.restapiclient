using System;
using Cortside.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers;

namespace Cortside.RestApiClient {
    public class JsonNetSerializer : IRestSerializer, ISerializer, IDeserializer {
        private readonly JsonSerializerSettings settings;

        public JsonNetSerializer() {
            settings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
            settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            settings.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'" });
            settings.Converters.Add(new IsoTimeSpanConverter());
        }

        public JsonNetSerializer(JsonSerializerSettings settings) {
            this.settings = settings;
        }

        public string Serialize(object obj) => JsonConvert.SerializeObject(obj, settings);

        public string Serialize(Parameter parameter) => Serialize(parameter.Value);

        public T Deserialize<T>(RestResponse response) => JsonConvert.DeserializeObject<T>(response.Content, settings);

        public string ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;

        public ISerializer Serializer => this;

        public IDeserializer Deserializer => this;

        public string[] SupportedContentTypes => RestSharp.Serializers.ContentType.JsonAccept;

        public string[] AcceptedContentTypes => RestSharp.Serializers.ContentType.JsonAccept;

        public SupportsContentType SupportsContentType => contentType => contentType.EndsWith("json", StringComparison.InvariantCultureIgnoreCase);
    }
}
