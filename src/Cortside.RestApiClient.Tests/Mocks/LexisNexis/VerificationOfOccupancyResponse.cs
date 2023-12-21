using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cortside.RestApiClient.Tests.Mocks.LexisNexis {
    [XmlRoot(ElementName = "Header")]
    public class Header {
        [XmlElement(ElementName = "QueryId")]
        public string QueryId { get; set; }
        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }
        public string TransactionId { get; set; }
    }

    [XmlRoot(ElementName = "Name")]
    public class Name {
        [XmlElement(ElementName = "First")]
        public string First { get; set; }
        [XmlElement(ElementName = "Last")]
        public string Last { get; set; }
    }

    [XmlRoot(ElementName = "Address")]
    public class Address {
        [XmlElement(ElementName = "Zip5")]
        public string Zip5 { get; set; }
    }

    [XmlRoot(ElementName = "InputEcho")]
    public class InputEcho {
        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; }
        [XmlElement(ElementName = "Address")]
        public Address Address { get; set; }
        [XmlElement(ElementName = "Phone")]
        public string Phone { get; set; }
        [XmlElement(ElementName = "SSN")]
        public string SSN { get; set; }
    }

    [XmlRoot(ElementName = "Attribute")]
    public class Attribute {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "Attributes")]
    public class Attributes {
        [XmlElement(ElementName = "Attribute")]
        public List<Attribute> Attribute { get; set; }
    }

    [XmlRoot(ElementName = "AttributeGroup")]
    public class AttributeGroup {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Attributes")]
        public Attributes Attributes { get; set; }
    }

    [XmlRoot(ElementName = "Result")]
    public class Result {
        [XmlElement(ElementName = "InputEcho")]
        public InputEcho InputEcho { get; set; }
        [XmlElement(ElementName = "UniqueId")]
        public string UniqueId { get; set; }
        [XmlElement(ElementName = "AttributeGroup")]
        public AttributeGroup AttributeGroup { get; set; }
    }

    [XmlRoot(ElementName = "response")]
    public class ResponseEx {
        [XmlElement(ElementName = "Header")]
        public Header Header { get; set; }
        [XmlElement(ElementName = "Result")]
        public Result Result { get; set; }
    }

    [XmlRoot(ElementName = "VerificationOfOccupancyResponse")]
    public class VerificationOfOccupancyResponse {
        [XmlElement(ElementName = "response")]
        public ResponseEx Response { get; set; }
    }
}
