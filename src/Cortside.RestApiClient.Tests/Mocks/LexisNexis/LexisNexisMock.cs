using System;
using System.Text.RegularExpressions;
using Cortside.MockServer;
using Cortside.MockServer.Builder;
using Cortside.RestApiClient.Tests.ResponseProviders;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Cortside.RestApiClient.Tests.Mocks.LexisNexis {
    public class LexisNexisMock : IMockHttpMock {
        public void Configure(MockHttpServer server) {
            // generate response depending on what tester requests in street1 field
            server.WireMockServer
                .Given(
                Request.Create().UsingPost()
                    .WithPath("/WsIdentity/InstantID")
                    .WithHeader("Authorization", "Basic Zm9vOmJhcg==") //base64 encoding of `foo:bar`
                )
                .AtPriority(1000) // set low to allow overrides by json mappings
                .RespondWith(
                    Response.Create()
                        .WithBody(r => InstantId.BuildResponse(r.Body))
                        .WithHeader("Content-Type", "text/xml")
                );

            // generate response depending on what tester requests in street1 field
            server.WireMockServer
                .Given(
                    Request.Create().UsingPost()
                        .WithPath("/WsIdentity/VerificationOfOccupancy")
                )
                .AtPriority(1000) // set low to allow overrides by json mappings
                .RespondWith(
                    Response.Create()
                        .WithBody(r => BuildResponse(r.Body))
                        .WithHeader("Content-Type", "text/xml")
                );
        }

        public static string BuildResponse(string requestBody) {
            // default values, which pass ownership & occupancy
            var own = "6";
            var occ = "6";

            // testers can enter their requested values in the street1 address field
            var userRequest = Regex.Match(requestBody, "&SearchBy.Address.StreetAddress1=.*&").Value;
            userRequest = Uri.UnescapeDataString(userRequest);
            Console.WriteLine($"[{DateTime.Now}] VOO Street1 matcher found: {userRequest}");

            if (userRequest.Contains("own=", StringComparison.CurrentCultureIgnoreCase)) {
                // set own score if request specifies
                own = Regex.Match(userRequest.ToLower(), @"(?<=own\=)\d+").Value;
                Console.WriteLine($"New own value: {own}");
            }

            if (userRequest.Contains("occ=", StringComparison.CurrentCultureIgnoreCase)) {
                // set occ score if request specifies
                occ = Regex.Match(userRequest.ToLower(), @"(?<=occ\=)\d+").Value;
                Console.WriteLine($"New occ value: {occ}");
            }

            var responseBody = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><VerificationOfOccupancyResponse><response><Header><Status>0</Status></Header><Result><InputEcho><Name><First>Jose</First><Last>Martinez</Last></Name><Address><Zip5>21061</Zip5></Address><Phone>4075046755</Phone><SSN>023687742</SSN></InputEcho><UniqueId>0</UniqueId><AttributeGroup><Name>VOOATTRV1</Name><Attributes><Attribute><Name>AddressReportingSourceIndex</Name><Value>3</Value></Attribute><Attribute><Name>AddressReportingHistoryIndex</Name><Value>{occ}</Value></Attribute><Attribute><Name>AddressSearchHistoryIndex</Name><Value>5</Value></Attribute><Attribute><Name>AddressUtilityHistoryIndex</Name><Value>1</Value></Attribute><Attribute><Name>AddressOwnershipHistoryIndex</Name><Value>{own}</Value></Attribute><Attribute><Name>AddressPropertyTypeIndex</Name><Value>5</Value></Attribute><Attribute><Name>AddressValidityIndex</Name><Value>4</Value></Attribute><Attribute><Name>RelativesConfirmingAddressIndex</Name><Value>4</Value></Attribute><Attribute><Name>AddressOwnerMailingAddressIndex</Name><Value>6</Value></Attribute><Attribute><Name>PriorAddressMoveIndex</Name><Value>8</Value></Attribute><Attribute><Name>PriorResidentMoveIndex</Name><Value>0</Value></Attribute><Attribute><Name>AddressDateFirstSeen</Name><Value>199907</Value></Attribute><Attribute><Name>AddressDateLastSeen</Name><Value>201403</Value></Attribute><Attribute><Name>OccupancyOverride</Name><Value>0</Value></Attribute><Attribute><Name>InferredOwnershipTypeIndex</Name><Value>4</Value></Attribute><Attribute><Name>OtherOwnedPropertyProximity</Name><Value>0</Value></Attribute><Attribute><Name>VerificationOfOccupancyScore</Name><Value>70</Value></Attribute></Attributes></AttributeGroup></Result></response></VerificationOfOccupancyResponse>";

            return responseBody;
        }
    }
}
