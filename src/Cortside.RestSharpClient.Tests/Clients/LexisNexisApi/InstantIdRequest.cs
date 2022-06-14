using System.Xml.Serialization;

// name space is intentionally off so as not to conflict with xmlelements in response file
namespace Cortside.RestSharpClient.Tests.Clients.LexisNexisApi {
    [XmlRoot(ElementName = "User")]
    public class User {
        [XmlElement(ElementName = "GLBPurpose")]
        public string GLBPurpose { get; set; }
        [XmlElement(ElementName = "DLPurpose")]
        public string DLPurpose { get; set; }
        public string AccountNumber { get; set; }
        public int MaxWaitSeconds { get; set; }
        public bool MaxWaitSecondsSpecified { get; set; }
        public string QueryId { get; set; }
        public string ReferenceCode { get; set; }
    }

    [XmlRoot(ElementName = "WatchLists")]
    public class WatchLists {
        [XmlElement(ElementName = "WatchList")]
        public string WatchList { get; set; }
    }

    [XmlRoot(ElementName = "FraudPointModel")]
    public class FraudPointModel {
        [XmlElement(ElementName = "IncludeRiskIndices")]
        public string IncludeRiskIndices { get; set; }
    }

    [XmlRoot(ElementName = "IncludeModels")]
    public class IncludeModels {
        [XmlElement(ElementName = "FraudPointModel")]
        public FraudPointModel FraudPointModel { get; set; }
    }

    [XmlRoot(ElementName = "DOBMatch")]
    public class DobMatch {
        [XmlElement(ElementName = "MatchType")]
        public string MatchType { get; set; }
        public int MatchYearRadius { get; set; }
        public bool MatchYearRadiusSpecified { get; set; }
    }

    [XmlRoot(ElementName = "CVICalculationOptions")]
    public class CviCalculationOptions {
        [XmlElement(ElementName = "IncludeDOB")]
        public string IncludeDOB { get; set; }
        [XmlElement(ElementName = "IncludeDriverLicense")]
        public string IncludeDriverLicense { get; set; }
    }

    [XmlRoot(ElementName = "Options")]
    public class Options {
        [XmlElement(ElementName = "WatchLists")]
        public WatchLists WatchLists { get; set; }
        [XmlElement(ElementName = "IncludeCLOverride")]
        public string IncludeCLOverride { get; set; }
        [XmlElement(ElementName = "IncludeMSOverride")]
        public string IncludeMSOverride { get; set; }
        [XmlElement(ElementName = "IncludeDLVerification")]
        public string IncludeDLVerification { get; set; }
        [XmlElement(ElementName = "PoBoxCompliance")]
        public string PoBoxCompliance { get; set; }
        [XmlElement(ElementName = "IncludeModels")]
        public IncludeModels IncludeModels { get; set; }
        [XmlElement(ElementName = "DOBMatch")]
        public DobMatch DOBMatch { get; set; }
        [XmlElement(ElementName = "IncludeAllRiskIndicators")]
        public bool IncludeAllRiskIndicators { get; set; }
        [XmlElement(ElementName = "IncludeMIOverride")]
        public string IncludeMIOverride { get; set; }
        [XmlElement(ElementName = "CVICalculationOptions")]
        public CviCalculationOptions CVICalculationOptions { get; set; }
        public string InstantIDVersion { get; set; }
    }

    [XmlRoot(ElementName = "Name")]
    public class Name {
        [XmlElement(ElementName = "First")]
        public string First { get; set; }
        [XmlElement(ElementName = "Last")]
        public string Last { get; set; }
        public string Suffix { get; set; }
    }

    [XmlRoot(ElementName = "Address")]
    public class Address {
        [XmlElement(ElementName = "StreetAddress1")]
        public string StreetAddress1 { get; set; }
        [XmlElement(ElementName = "City")]
        public string City { get; set; }
        [XmlElement(ElementName = "State")]
        public string State { get; set; }
        [XmlElement(ElementName = "Zip5")]
        public string Zip5 { get; set; }
        public string StreetAddress2 { get; set; }
    }

    [XmlRoot(ElementName = "SearchBy")]
    public class SearchBy {
        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; }
        [XmlElement(ElementName = "Address")]
        public Address Address { get; set; }
        [XmlElement(ElementName = "SSN")]
        public string SSN { get; set; }
        [XmlElement(ElementName = "HomePhone")]
        public string HomePhone { get; set; }
        public bool UseDOBFilter { get; set; }
        public bool UseDOBFilterSpecified { get; set; }
        public int DOBRadius { get; set; }
        public bool DOBRadiusSpecified { get; set; }

        public Date DOB { get; set; }
    }

    [XmlRoot(ElementName = "InstantIDRequest")]
    public class InstantIDRequest {
        [XmlElement(ElementName = "User")]
        public User User { get; set; }
        [XmlElement(ElementName = "Options")]
        public Options Options { get; set; }
        [XmlElement(ElementName = "SearchBy")]
        public SearchBy SearchBy { get; set; }
    }

    public class Date {
        public short Year { get; set; }
        public bool YearSpecified { get; set; }
        public short Month { get; set; }
        public bool MonthSpecified { get; set; }
        public short Day { get; set; }
        public bool DaySpecified { get; set; }
    }
}
