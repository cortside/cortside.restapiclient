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
            // these values should match Cortside.AspNetCore JsonNetUtility
            settings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,

                // datetime specific handling
                // always output ISO-8601 format
                DateFormatHandling = DateFormatHandling.IsoDateFormat,

                // parse using DateTimeOffset so that ISO-8601 with timezone is used
                DateParseHandling = DateParseHandling.DateTimeOffset,

                // setting to control how DateTime and DateTimeOffset are serialized.
                // always serialize to utc
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            settings.Converters.Add(new IsoTimeSpanConverter());
        }

        public JsonNetSerializer(JsonSerializerSettings settings) {
            this.settings = settings;
        }

        public string Serialize(object obj) => JsonConvert.SerializeObject(obj, settings);

        public string Serialize(Parameter parameter) => Serialize(parameter?.Value);

        public T Deserialize<T>(RestResponse response) => JsonConvert.DeserializeObject<T>(response.Content, settings);

        public ContentType ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;

        public ISerializer Serializer => this;

        public IDeserializer Deserializer => this;

        public string[] SupportedContentTypes => ContentType.JsonAccept;

        public string[] AcceptedContentTypes => ContentType.JsonAccept;

        public SupportsContentType SupportsContentType => contentType => contentType.Value.EndsWith("json", StringComparison.InvariantCultureIgnoreCase);
    }
}
