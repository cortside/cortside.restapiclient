using System;
using System.Text.RegularExpressions;

namespace Cortside.RestApiClient.Tests.ResponseProviders {
    public static class InstantId {
        public static string BuildResponse(string requestBody) {
            // default values, which should NOT trigger questions
            var cvi = "50";
            var nas = "12";
            var nap = "12";
            var riskCodeSection = "";

            // testers can enter their requested values in the street1 address field
            var userRequest = Regex.Match(requestBody, "&SearchBy.Address.StreetAddress1=.*&").Value;
            userRequest = Uri.UnescapeDataString(userRequest);
            Console.WriteLine($"[{DateTime.Now}] InstantID street1 matcher found: {userRequest}");

            if (userRequest.Contains("cvi=", StringComparison.CurrentCultureIgnoreCase)) {
                // set cvi if request specifies
                cvi = Regex.Match(userRequest.ToLower(), @"(?<=cvi\=)\d+").Value;
                Console.WriteLine($"New cvi value: {cvi}");
            }

            if (userRequest.Contains("nas=", StringComparison.CurrentCultureIgnoreCase)) {
                // set nas if request specifies
                nas = Regex.Match(userRequest.ToLower(), @"(?<=nas\=)\d+").Value;
                Console.WriteLine($"New nas value: {nas}");
            }

            if (userRequest.Contains("nap=", StringComparison.CurrentCultureIgnoreCase)) {
                // set nap if request specifies
                nap = Regex.Match(userRequest.ToLower(), @"(?<=nap\=)\d+").Value;
                Console.WriteLine($"New nap value: {nap}");
            }

            if (userRequest.Contains("riskcodes=", StringComparison.CurrentCultureIgnoreCase)) {
                // multiple risk codes can be specified, in a comma separated list.  Codes can be alphanumeric.
                riskCodeSection = GetCustomRiskCodeXmlForResponse(userRequest);
            }

            var responseBody = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><InstantIDResponseEx><response><Header><Status>0</Status><TransactionId>41889</TransactionId></Header><Result><InputEcho><Name><First>Lee</First><Last>Colon</Last></Name><Address><StreetAddress1>1600 HILLSIDE DR</StreetAddress1><City>BAKERSFIELD</City><State>CA</State><Zip5>93306</Zip5></Address><DOB><Year>1955</Year><Month>1</Month><Day>1</Day></DOB><SSN>469197591</SSN><HomePhone>9414625524</HomePhone><UseDOBFilter>1</UseDOBFilter><DOBRadius>1</DOBRadius><Passport><Number></Number><ExpirationDate/><Country></Country><MachineReadableLine1></MachineReadableLine1><MachineReadableLine2></MachineReadableLine2></Passport><Channel></Channel><OwnOrRent></OwnOrRent></InputEcho><UniqueId>385133841</UniqueId><NameAddressSSNSummary>{nas}</NameAddressSSNSummary><AdditionalScore1>0</AdditionalScore1><AdditionalScore2>0</AdditionalScore2><VerifiedInput><SSN>46919xxxx</SSN><HomePhone>9414625524</HomePhone><Name><First>LEE</First><Last>COLON</Last></Name><DOB><Year>1971</Year><Month>12</Month><Day>XX</Day></DOB></VerifiedInput><SSNInfo><Valid>G</Valid><IssuedLocation>NEW YORK</IssuedLocation><IssuedEndDate><Year>1951</Year><Month>12</Month></IssuedEndDate><IssuedStartDate><Year>1936</Year><Month>01</Month></IssuedStartDate></SSNInfo><CurrentName><First>LEE</First><Last>COLON</Last></CurrentName><ReversePhone><Name><First>LEE</First><Last>COLON</Last></Name><Address><StreetAddress1>1600 HILLSIDE DR</StreetAddress1><City>BAKERSFIELD</City><State>CA</State><Zip5>93306</Zip5><StreetNumber>1600</StreetNumber><StreetName>HILLSIDE</StreetName><StreetSuffix>DR</StreetSuffix></Address></ReversePhone><NameAddressPhone><Summary>{nap}</Summary><Type>P</Type></NameAddressPhone><ComprehensiveVerification><ComprehensiveVerificationIndex>{cvi}</ComprehensiveVerificationIndex><RiskIndicators>{riskCodeSection}</RiskIndicators><PotentialFollowupActions><FollowupAction><RiskCode>C</RiskCode><Description>Verify name with Address (via DL  utility bill  Directory Assistance  paycheck stub  or other Govern</Description></FollowupAction><FollowupAction><RiskCode>D</RiskCode><Description>Verify phone (Directory Assistance  utility bill)</Description></FollowupAction></PotentialFollowupActions></ComprehensiveVerification><PassportValidated>0</PassportValidated><AddressPOBox>false</AddressPOBox><AddressCMRA>false</AddressCMRA><SSNFoundForLexID>false</SSNFoundForLexID><DOBMatchLevel>8</DOBMatchLevel><InstantIDVersion>1</InstantIDVersion><DOBVerified>1</DOBVerified><ChronologyHistories><ChronologyHistory><Address><StreetAddress1>1600 HILLSIDE DR</StreetAddress1><City>BALLWIN</City><State>MO</State><Zip5>63011</Zip5><StreetSuffix>DR</StreetSuffix><StreetName>HILLSIDE</StreetName><StreetNumber>1600</StreetNumber></Address><DateFirstSeen><Year>1985</Year><Month>09</Month></DateFirstSeen><DateLastSeen><Year>2013</Year><Month>09</Month></DateLastSeen></ChronologyHistory><ChronologyHistory><Phone>6362271288</Phone><Address><StreetAddress1>498 BAYWILLOW DR</StreetAddress1><City>BALLWIN</City><State>MO</State><Zip5>63011</Zip5><Zip4>3421</Zip4></Address><IsBestAddress>1</IsBestAddress><DateFirstSeen><Year>1996</Year><Month>07</Month></DateFirstSeen><DateLastSeen><Year>2013</Year><Month>09</Month></DateLastSeen></ChronologyHistory><ChronologyHistory><Phone>6362271288</Phone><Address><StreetAddress1>493 BAYWILLOW DR</StreetAddress1><City>BALLWIN</City><State>MO</State><Zip5>63011</Zip5><Zip4>3420</Zip4></Address><DateFirstSeen><Year>1989</Year><Month>12</Month></DateFirstSeen><DateLastSeen><Year>2013</Year><Month>09</Month></DateLastSeen></ChronologyHistory></ChronologyHistories></Result></response></InstantIDResponseEx>";
            return responseBody;
        }

        private static string GetCustomRiskCodeXmlForResponse(string userRequest) {
            var codes = Regex.Match(userRequest.ToLower(), @"(?<=riskcodes\=).*;").Value; // ending ; is required
            Console.WriteLine($"Risk codes requested by user: {codes}");

            codes = codes.Split(";")[0]; // snag only stuff before the semi-colon.   Up to user to make sure they add the ending semi-colon for the risk codes list.

            var riskCodeXml = "";
            var sequence = 1;

            foreach (var code in codes.Split(",")) {
                if (!string.IsNullOrEmpty(code.Trim())) {
                    riskCodeXml += $"<RiskIndicator><RiskCode>{code.ToUpper().Trim()}</RiskCode><Description>Generated by Cortside's LexisNexis.WireMock</Description><Sequence>{sequence}</Sequence></RiskIndicator>";
                    sequence++;
                }
            }

            Console.WriteLine($"RiskCode Xml generated: {riskCodeXml}");
            return riskCodeXml;
        }
    }
}
