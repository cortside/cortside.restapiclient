using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cortside.RestSharpClient.Tests.Clients.LexisNexisApi {
    [XmlRoot(ElementName = "Header")]
    public class Header {
        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }
        [XmlElement(ElementName = "TransactionId")]
        public string TransactionId { get; set; }
    }

    [XmlRoot(ElementName = "DOB")]
    public class Dob {
        [XmlElement(ElementName = "Year")]
        public string Year { get; set; }
        [XmlElement(ElementName = "Month")]
        public string Month { get; set; }
        [XmlElement(ElementName = "Day")]
        public string Day { get; set; }
    }

    [XmlRoot(ElementName = "Passport")]
    public class Passport {
        [XmlElement(ElementName = "Number")]
        public string Number { get; set; }
        [XmlElement(ElementName = "ExpirationDate")]
        public string ExpirationDate { get; set; }
        [XmlElement(ElementName = "Country")]
        public string Country { get; set; }
        [XmlElement(ElementName = "MachineReadableLine1")]
        public string MachineReadableLine1 { get; set; }
        [XmlElement(ElementName = "MachineReadableLine2")]
        public string MachineReadableLine2 { get; set; }
    }

    [XmlRoot(ElementName = "InputEcho")]
    public class InputEcho {
        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; }
        [XmlElement(ElementName = "Address")]
        public Address Address { get; set; }
        [XmlElement(ElementName = "DOB")]
        public Dob DOB { get; set; }
        [XmlElement(ElementName = "SSN")]
        public string SSN { get; set; }
        [XmlElement(ElementName = "HomePhone")]
        public string HomePhone { get; set; }
        [XmlElement(ElementName = "UseDOBFilter")]
        public string UseDOBFilter { get; set; }
        [XmlElement(ElementName = "DOBRadius")]
        public string DOBRadius { get; set; }
        [XmlElement(ElementName = "Passport")]
        public Passport Passport { get; set; }
        [XmlElement(ElementName = "Channel")]
        public string Channel { get; set; }
        [XmlElement(ElementName = "OwnOrRent")]
        public string OwnOrRent { get; set; }
    }

    [XmlRoot(ElementName = "RiskIndicator")]
    public class RiskIndicator {
        [XmlElement(ElementName = "RiskCode")]
        public string RiskCode { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "Sequence")]
        public string Sequence { get; set; }
    }

    [XmlRoot(ElementName = "RiskIndicators")]
    public class RiskIndicators {
        [XmlElement(ElementName = "RiskIndicator")]
        public List<RiskIndicator> RiskIndicator { get; set; }
    }

    [XmlRoot(ElementName = "FollowupAction")]
    public class FollowupAction {
        [XmlElement(ElementName = "RiskCode")]
        public string RiskCode { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }

    [XmlRoot(ElementName = "PotentialFollowupActions")]
    public class PotentialFollowupActions {
        [XmlElement(ElementName = "FollowupAction")]
        public List<FollowupAction> FollowupAction { get; set; }
    }

    [XmlRoot(ElementName = "ComprehensiveVerification")]
    public class ComprehensiveVerification {
        [XmlElement(ElementName = "ComprehensiveVerificationIndex")]
        public int ComprehensiveVerificationIndex { get; set; }
        [XmlElement(ElementName = "RiskIndicators")]
        public RiskIndicators RiskIndicators { get; set; }
        [XmlElement(ElementName = "PotentialFollowupActions")]
        public PotentialFollowupActions PotentialFollowupActions { get; set; }
    }

    [XmlRoot(ElementName = "NameAddressPhone")]
    public class NameAddressPhone {
        [XmlElement(ElementName = "Summary")]
        public string Summary { get; set; }
        [XmlElement(ElementName = "Type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "VerifiedInput")]
    public class VerifiedInput {
        [XmlElement(ElementName = "SSN")]
        public string SSN { get; set; }
        [XmlElement(ElementName = "HomePhone")]
        public string HomePhone { get; set; }
        [XmlElement(ElementName = "DOB")]
        public Dob DOB { get; set; }
        [XmlElement(ElementName = "Address")]
        public Address Address { get; set; }
        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; }
    }

    [XmlRoot(ElementName = "CurrentName")]
    public class CurrentName {
        [XmlElement(ElementName = "First")]
        public string First { get; set; }
        [XmlElement(ElementName = "Last")]
        public string Last { get; set; }
    }

    [XmlRoot(ElementName = "IssuedEndDate")]
    public class IssuedEndDate {
        [XmlElement(ElementName = "Year")]
        public string Year { get; set; }
        [XmlElement(ElementName = "Month")]
        public string Month { get; set; }
    }

    [XmlRoot(ElementName = "IssuedStartDate")]
    public class IssuedStartDate {
        [XmlElement(ElementName = "Year")]
        public string Year { get; set; }
        [XmlElement(ElementName = "Month")]
        public string Month { get; set; }
    }

    [XmlRoot(ElementName = "SSNInfo")]
    public class SsnInfo {
        [XmlElement(ElementName = "Valid")]
        public string Valid { get; set; }
        [XmlElement(ElementName = "IssuedLocation")]
        public string IssuedLocation { get; set; }
        [XmlElement(ElementName = "IssuedEndDate")]
        public IssuedEndDate IssuedEndDate { get; set; }
        [XmlElement(ElementName = "IssuedStartDate")]
        public IssuedStartDate IssuedStartDate { get; set; }
    }

    [XmlRoot(ElementName = "ReversePhone")]
    public class ReversePhone {
        [XmlElement(ElementName = "Address")]
        public Address Address { get; set; }
        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; }
    }

    [XmlRoot(ElementName = "DateFirstSeen")]
    public class DateFirstSeen {
        [XmlElement(ElementName = "Year")]
        public string Year { get; set; }
        [XmlElement(ElementName = "Month")]
        public string Month { get; set; }
    }

    [XmlRoot(ElementName = "DateLastSeen")]
    public class DateLastSeen {
        [XmlElement(ElementName = "Year")]
        public string Year { get; set; }
        [XmlElement(ElementName = "Month")]
        public string Month { get; set; }
    }

    [XmlRoot(ElementName = "ChronologyHistory")]
    public class ChronologyHistory {
        [XmlElement(ElementName = "Address")]
        public Address Address { get; set; }
        [XmlElement(ElementName = "DateFirstSeen")]
        public DateFirstSeen DateFirstSeen { get; set; }
        [XmlElement(ElementName = "DateLastSeen")]
        public DateLastSeen DateLastSeen { get; set; }
        [XmlElement(ElementName = "IsBestAddress")]
        public string IsBestAddress { get; set; }
        [XmlElement(ElementName = "Phone")]
        public string Phone { get; set; }
    }

    [XmlRoot(ElementName = "ChronologyHistories")]
    public class ChronologyHistories {
        [XmlElement(ElementName = "ChronologyHistory")]
        public List<ChronologyHistory> ChronologyHistory { get; set; }
    }

    [XmlRoot(ElementName = "Result")]
    public class Result {
        [XmlElement(ElementName = "InputEcho")]
        public InputEcho InputEcho { get; set; }
        [XmlElement(ElementName = "UniqueId")]
        public string UniqueId { get; set; }
        [XmlElement(ElementName = "NameAddressSSNSummary")]
        public int NameAddressSSNSummary { get; set; }
        [XmlElement(ElementName = "AdditionalScore1")]
        public string AdditionalScore1 { get; set; }
        [XmlElement(ElementName = "AdditionalScore2")]
        public string AdditionalScore2 { get; set; }
        [XmlElement(ElementName = "PhoneOfNameAddress")]
        public string PhoneOfNameAddress { get; set; }
        [XmlElement(ElementName = "ComprehensiveVerification")]
        public ComprehensiveVerification ComprehensiveVerification { get; set; }
        [XmlElement(ElementName = "NameAddressPhone")]
        public NameAddressPhone NameAddressPhone { get; set; }
        [XmlElement(ElementName = "VerifiedInput")]
        public VerifiedInput VerifiedInput { get; set; }
        [XmlElement(ElementName = "DOBVerified")]
        public string DOBVerified { get; set; }
        [XmlElement(ElementName = "PassportValidated")]
        public string PassportValidated { get; set; }
        [XmlElement(ElementName = "DOBMatchLevel")]
        public string DOBMatchLevel { get; set; }
        [XmlElement(ElementName = "SSNFoundForLexID")]
        public string SSNFoundForLexID { get; set; }
        [XmlElement(ElementName = "AddressPOBox")]
        public string AddressPOBox { get; set; }
        [XmlElement(ElementName = "AddressCMRA")]
        public string AddressCMRA { get; set; }
        [XmlElement(ElementName = "InstantIDVersion")]
        public string InstantIDVersion { get; set; }
        [XmlElement(ElementName = "CurrentName")]
        public CurrentName CurrentName { get; set; }
        [XmlElement(ElementName = "SSNInfo")]
        public SsnInfo SSNInfo { get; set; }
        [XmlElement(ElementName = "ReversePhone")]
        public ReversePhone ReversePhone { get; set; }
        [XmlElement(ElementName = "ChronologyHistories")]
        public ChronologyHistories ChronologyHistories { get; set; }
    }

    [XmlRoot(ElementName = "response")]
    public class Response {
        [XmlElement(ElementName = "Header")]
        public Header Header { get; set; }
        [XmlElement(ElementName = "Result")]
        public Result Result { get; set; }
    }

    [XmlRoot(ElementName = "InstantIDResponseEx")]
    public class InstantIDResponseEx {
        [XmlElement(ElementName = "response")]
        public Response Response { get; set; }
    }
}
