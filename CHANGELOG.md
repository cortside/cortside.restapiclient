# Release 6.1

|Commit|Date|Author|Message|
|---|---|---|---|
| f3774da | <span style="white-space:nowrap;">2023-08-30</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update version
| fcc1c4b | <span style="white-space:nowrap;">2023-09-04</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| a5ad76b | <span style="white-space:nowrap;">2023-09-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-18] use serializer instead of ToString() for parameter conversion
| df4e215 | <span style="white-space:nowrap;">2023-09-11</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-18] use scoped timezone
| 5532565 | <span style="white-space:nowrap;">2023-09-11</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-18] use scoped timezone to test case
| 50a3e35 | <span style="white-space:nowrap;">2023-09-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-18] cleanup lint warnings; use new Cortside.Common.Testing library
| 5649c0a | <span style="white-space:nowrap;">2023-09-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-18] cleanup lint warnings; unskip tests
| 53bd98e | <span style="white-space:nowrap;">2023-09-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-18] cleanup lint warnings; unskip tests
| 38aeb50 | <span style="white-space:nowrap;">2023-09-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/ISSUE-18, ISSUE-18) [ISSUE-18] cleanup lint warnings; unskip tests
| 37a702c | <span style="white-space:nowrap;">2023-09-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #19 from cortside/ISSUE-18
| 69b9e16 | <span style="white-space:nowrap;">2023-09-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add more error logging for things like cert errors and more consistent exception only when options are set to throw
| 50b1fee | <span style="white-space:nowrap;">2023-10-17</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-20] fail request when authenticator fails
| 8d0fdc5 | <span style="white-space:nowrap;">2023-10-18</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/ISSUE-20, ISSUE-20) [ISSUE-20] GetAsync should return a response, even if the status is not 200 so that caller can interrogate for error
| 4ab1b58 | <span style="white-space:nowrap;">2023-10-18</span> | <span style="white-space:nowrap;">gkingston-regions</span> |  Merge pull request #21 from cortside/ISSUE-20
| d64421f | <span style="white-space:nowrap;">2023-10-18</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Add extension method for easier logging of response failures
| 9de9478 | <span style="white-space:nowrap;">2023-11-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  resolve issue where returning of cached token did not return Bearer prefix
| 1746178 | <span style="white-space:nowrap;">2023-11-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  move servicecollection extension methods to register client to this project
| e4f960e | <span style="white-space:nowrap;">2023-11-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to latest nuget packages with required changes
| f588494 | <span style="white-space:nowrap;">2023-11-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  ignore ncrunch directories
| 5626228 | <span style="white-space:nowrap;">2023-11-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add support to use a byo-httpclient
| ece68df | <span style="white-space:nowrap;">2023-11-10</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  refactor authenticator to make it easier to understand and to help ensure that it's more likely thread-safe
| 0b4b461 | <span style="white-space:nowrap;">2023-11-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> release/6.1, origin/develop, origin/HEAD, develop) update changelog with release notes
****

# Release 6.1

* Update nuget dependencies to latest stable versions
* Change to use client serializer for request parameters, needed for proper date/time formatting if something other than default ToString() functionality is desired
* Add ServiceCollection extension method to register a RestApiClient based implemenation client
	```csharp
	var clientConfig = configuration.GetSection("CatalogApi").Get<CatalogClientConfiguration>();
	clientConfig.Authentication = idsConfig.Authentication;	
	services.AddRestApiClient<ICatalogClient, CatalogClient, CatalogClientConfiguration>(clientConfig);
	```
* OpenIDConnectAuthenticator now caches access token and expiration to save on authentication requests	
* Client should now fail request if there is an authenticator and authentication fails (instead of silently trying the request and failing on authorization)
* Fix handling of exceptions when ThrowOnAnyError or ThrowOnDeserializationError is false
* Add RestResponse extension method LoggedFailureException to make it easier to log an error and throw an exception on unsuccessful request
	```csharp
	if (!response.IsSuccessful) {
		throw response.LoggedFailureException(logger, "Error contacting catalog api to retrieve item info for {0}", sku);
	}
	```
* Add option to use a bring-your-own-httpclient
	```csharp
	var httpClient = server.CreateClient();
	var client = new RestApiClient(new NullLogger<RestApiClient>(), new HttpContextAccessor(), new RestApiClientOptions(), httpClient);
	```	
* Code cleanup addressing lint warnings as well as adding additional tests

|Commit|Date|Author|Message|
|---|---|---|---|
| b069743 | <span style="white-space:nowrap;">2023-08-30</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  generate changelog
| f3774da | <span style="white-space:nowrap;">2023-08-30</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update version
| 1310f6f | <span style="white-space:nowrap;">2023-08-30</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update release notes
| 2428e33 | <span style="white-space:nowrap;">2023-08-30</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (release/6.0) update release notes
| 1cbbe77 | <span style="white-space:nowrap;">2023-08-30</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/release/6.0, origin/master) Merge pull request #17 from cortside/release/6.0
| fcc1c4b | <span style="white-space:nowrap;">2023-09-04</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| a5ad76b | <span style="white-space:nowrap;">2023-09-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-18] use serializer instead of ToString() for parameter conversion
| df4e215 | <span style="white-space:nowrap;">2023-09-11</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-18] use scoped timezone
| 5532565 | <span style="white-space:nowrap;">2023-09-11</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-18] use scoped timezone to test case
| 50a3e35 | <span style="white-space:nowrap;">2023-09-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-18] cleanup lint warnings; use new Cortside.Common.Testing library
| 5649c0a | <span style="white-space:nowrap;">2023-09-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-18] cleanup lint warnings; unskip tests
| 53bd98e | <span style="white-space:nowrap;">2023-09-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-18] cleanup lint warnings; unskip tests
| 38aeb50 | <span style="white-space:nowrap;">2023-09-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/ISSUE-18, ISSUE-18) [ISSUE-18] cleanup lint warnings; unskip tests
| 37a702c | <span style="white-space:nowrap;">2023-09-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #19 from cortside/ISSUE-18
| 69b9e16 | <span style="white-space:nowrap;">2023-09-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add more error logging for things like cert errors and more consistent exception only when options are set to throw
| 50b1fee | <span style="white-space:nowrap;">2023-10-17</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ISSUE-20] fail request when authenticator fails
| 8d0fdc5 | <span style="white-space:nowrap;">2023-10-18</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/ISSUE-20, ISSUE-20) [ISSUE-20] GetAsync should return a response, even if the status is not 200 so that caller can interrogate for error
| 4ab1b58 | <span style="white-space:nowrap;">2023-10-18</span> | <span style="white-space:nowrap;">gkingston-regions</span> |  Merge pull request #21 from cortside/ISSUE-20
| d64421f | <span style="white-space:nowrap;">2023-10-18</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Add extension method for easier logging of response failures
| 9de9478 | <span style="white-space:nowrap;">2023-11-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  resolve issue where returning of cached token did not return Bearer prefix
| 1746178 | <span style="white-space:nowrap;">2023-11-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  move servicecollection extension methods to register client to this project
| e4f960e | <span style="white-space:nowrap;">2023-11-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to latest nuget packages with required changes
| f588494 | <span style="white-space:nowrap;">2023-11-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> develop, origin/develop, origin/HEAD) ignore ncrunch directories
****
# Release 6.0

* Update version number to match framework version (6.x)
* Update projects to be net6.0
* Update nuget dependencies to latest stable versions
* Update to latest RestSharp and handle breaking changes
* Set X-Forwarded-For header on client requests that originated from httprequest to capture original client ip
* Improved authenticator logging
* Handle deserialization exception when throw is true

|Commit|Date|Author|Message|
|---|---|---|---|
| 6fb4e8b | <span style="white-space:nowrap;">2023-06-15</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update version
| c28b6e9 | <span style="white-space:nowrap;">2023-06-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| 1400841 | <span style="white-space:nowrap;">2023-06-23</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] update to net6
| bb7b232 | <span style="white-space:nowrap;">2023-06-23</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] update to latest restsharp library
| d9744b8 | <span style="white-space:nowrap;">2023-06-23</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [net6] use vs2022 builder image
| 5e0f21b | <span style="white-space:nowrap;">2023-06-23</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/net6, net6) [net6] add useful message to when token parsing is not successful
| 4c336fd | <span style="white-space:nowrap;">2023-06-23</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #15 from cortside/net6
| 541bc53 | <span style="white-space:nowrap;">2023-07-17</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update version to 6.x to be in line with dotnet and net6 version numbers
| 65ac8c9 | <span style="white-space:nowrap;">2023-07-17</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update version to 6.x to be in line with dotnet and net6 version numbers
| 85bbf9c | <span style="white-space:nowrap;">2023-07-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  set X-Forwarded-For header on client requests that originated from httprequest to capture original client ip
| 7a47485 | <span style="white-space:nowrap;">2023-07-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  set X-Forwarded-For header on client requests that originated from httprequest to capture original client ip
| 35d06d0 | <span style="white-space:nowrap;">2023-07-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  set X-Forwarded-For header on client requests that originated from httprequest to capture original client ip
| d96fafe | <span style="white-space:nowrap;">2023-07-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  allow for any ILogger in authenticator
| d66d8a4 | <span style="white-space:nowrap;">2023-07-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add more tests for delegation and better validation if there is reason to attempt to delegate
| 346d497 | <span style="white-space:nowrap;">2023-08-15</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [OR-2315] handle deserialization exception when throw is true
| 82eaaea | <span style="white-space:nowrap;">2023-08-15</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/OR-2315, OR-2315) nudge for build
| 5a21159 | <span style="white-space:nowrap;">2023-08-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #16 from cortside/OR-2315
| ecbcdd4 | <span style="white-space:nowrap;">2023-08-30</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> release/6.0, origin/develop, origin/HEAD, develop) update to latest nuget packages
****

# Release 1.2

|Commit|Date|Author|Message|
|---|---|---|---|
| 3d33f42 | <span style="white-space:nowrap;">2023-01-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update version
| ef6ff2c | <span style="white-space:nowrap;">2023-01-04</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'master' into develop
| 30759d7 | <span style="white-space:nowrap;">2023-03-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [ARC-105] add delegation handling to OpenIdConnectAuthenticator
| c4f309a | <span style="white-space:nowrap;">2023-03-22</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/ARC-105, ARC-105) [ARC-105] add additional tests to validate delegation workflow in oidc authentication
| bf74a89 | <span style="white-space:nowrap;">2023-03-23</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #12 from cortside/ARC-105
| bedb3a5 | <span style="white-space:nowrap;">2023-04-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/ARC-116, ARC-116) [ARC-116] check the status code for following redirects
| e42db2e | <span style="white-space:nowrap;">2023-04-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #13 from cortside/ARC-116
| e885581 | <span style="white-space:nowrap;">2023-06-15</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update powershell scripts
| 882b530 | <span style="white-space:nowrap;">2023-06-15</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> release/1.2, origin/develop, origin/HEAD, develop) update to latest cortside libraries
****

# Release 1.1

|Commit|Date|Author|Message|
|---|---|---|---|
| 97d077f | <span style="white-space:nowrap;">2022-07-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [feature/BOT-20220713] updated nuget packages
| 9fd9a43 | <span style="white-space:nowrap;">2022-07-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  handle git flow named branches
| 9eb54cc | <span style="white-space:nowrap;">2022-07-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #7 from cortside/feature/BOT-20220713
| 43c01d9 | <span style="white-space:nowrap;">2022-07-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [feature/BOT-20220713] updated nuget packages
| 005f0dc | <span style="white-space:nowrap;">2022-07-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [querybypost] add tests to ensure that 303 can be handled successfully
| e6e42b8 | <span style="white-space:nowrap;">2022-07-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [querybypost] override handling of 303 and 302 on POST
| 43498c5 | <span style="white-space:nowrap;">2022-07-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #8 from cortside/querybypost
| e481c40 | <span style="white-space:nowrap;">2022-07-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add the url for request to logged properties to make searching for responses easier
| 77faa7e | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add logging of request
| 6838eef | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  adjust logging
| 0145d06 | <span style="white-space:nowrap;">2022-12-12</span> | <span style="white-space:nowrap;">Glen Kingston</span> |  [SVC-2031] Changed token to be empty instead of null when needed
| bcbe694 | <span style="white-space:nowrap;">2022-12-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  reflect changes to RestSharp in determining response success
| d14927e | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  minor cleanup
| c5c2667 | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' into feature/SVC-2031
| 84ac55d | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Glen Kingston</span> |  [SVC-2031] Fixed test to check for empty due to new changes
| e8d6370 | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [SVC-2031] use constant instead of literal
| bd991de | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #9 from cortside/feature/SVC-2031
| 64c1cbe | <span style="white-space:nowrap;">2023-01-02</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [feature/BOT-20230102] updated nuget packages
| 0952253 | <span style="white-space:nowrap;">2023-01-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/feature/BOT-20230102, feature/BOT-20230102) update helper scripts; update nuget packages
| fe878b8 | <span style="white-space:nowrap;">2023-01-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #10 from cortside/feature/BOT-20230102
| 3790c57 | <span style="white-space:nowrap;">2023-01-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> release/1.1, origin/develop, origin/HEAD, develop) initial changelog
****

# Release 1.0
|Commit|Date|Author|Message|
|---|---|---|---|
| 2c1d168 | <span style="white-space:nowrap;">2022-01-24</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  initial version
| 8462cdf | <span style="white-space:nowrap;">2022-01-24</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  appveyor build
| e82ad93 | <span style="white-space:nowrap;">2022-01-24</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  appveyor build
| 7cd81e6 | <span style="white-space:nowrap;">2022-01-24</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  appveyor build
| 016970c | <span style="white-space:nowrap;">2022-01-24</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  appveyor build
| 6b6d01c | <span style="white-space:nowrap;">2022-01-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add authenticator for oidc
| 8494ae0 | <span style="white-space:nowrap;">2022-01-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add properties to set some things to minimize number of constructors
| 225f7bf | <span style="white-space:nowrap;">2022-01-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add overload for authenticator
| 5929463 | <span style="white-space:nowrap;">2022-01-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add support for custom HttpMessageHandler
| 81addba | <span style="white-space:nowrap;">2022-01-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  remove unneeded constructors
| 89167c6 | <span style="white-space:nowrap;">2022-02-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to latest restsharp
| dc02c16 | <span style="white-space:nowrap;">2022-03-04</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Update README.md
| 6b86e69 | <span style="white-space:nowrap;">2022-03-16</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Update README.md
| bdccfdb | <span style="white-space:nowrap;">2022-04-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #2 from cortside/develop
| d84c720 | <span style="white-space:nowrap;">2022-04-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to use duende version of demo ids
| 58dfbdf | <span style="white-space:nowrap;">2022-05-05</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add notes
| 7389c93 | <span style="white-space:nowrap;">2022-05-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to use sonarcloud correctly
| a010d92 | <span style="white-space:nowrap;">2022-05-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to use sonarcloud correctly
| aa32e7f | <span style="white-space:nowrap;">2022-05-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #4 from cortside/develop
| a03959c | <span style="white-space:nowrap;">2022-05-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget packages
| ecae726 | <span style="white-space:nowrap;">2022-05-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  retry wip
| 2c08d3f | <span style="white-space:nowrap;">2022-05-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] cleanup
| bb3949a | <span style="white-space:nowrap;">2022-05-30</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  get initial policies working with client and authenticator
| 0c8913a | <span style="white-space:nowrap;">2022-05-30</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add more tests and logging; cleanup;
| f92a5be | <span style="white-space:nowrap;">2022-05-30</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add method to follow redirects
| 41293b7 | <span style="white-space:nowrap;">2022-06-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add interfaces and create new request class to deal with shared client state
| b39e5bf | <span style="white-space:nowrap;">2022-06-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  use of RestApiClientOptions
| 8187af0 | <span style="white-space:nowrap;">2022-06-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update .editorconfig; resolve lint errors
| 53f2bda | <span style="white-space:nowrap;">2022-06-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  fix test
| a47a06c | <span style="white-space:nowrap;">2022-06-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to restsharp v108
| 9d54c63 | <span style="white-space:nowrap;">2022-06-07</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] wip addition of tests for options
| 5af3cea | <span style="white-space:nowrap;">2022-06-08</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] addition of tests for options
| f0d154b | <span style="white-space:nowrap;">2022-06-08</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] tests for RestApiRequest
| 5bc19dd | <span style="white-space:nowrap;">2022-06-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] remove FollowRedirect from base options and handle manually on client and request
| 7788418 | <span style="white-space:nowrap;">2022-06-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] remove FollowRedirect from base options and handle manually on client and request
| 11ccd55 | <span style="white-space:nowrap;">2022-06-09</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] reorder constructor arguments
| f906b7c | <span style="white-space:nowrap;">2022-06-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] add mockserver for additional test cases
| 8eb0ed7 | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] cleanup and tests; add test that shows usage of basic auth and xml serializer
| 0e161a9 | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] rename to RestApiClient
| 883b277 | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  rename project for sonar
| d892890 | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' into retry
| 1f8448c | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  fix sonar token
| 8960bec | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] fix test that asserted old name
| 1eaa89a | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  fix sonar token
| b371b12 | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  fix test that asserted old name
| 4a624b1 | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  rename project for sonar
| 862f84e | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  rename project for sonar
| 1d8d725 | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  merge from develop
| 429fc29 | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] add some request methods back that were extension methods; use consistent newtonsoft serializer with openId authenticator
| 6c583fe | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [retry] add some request methods back that were extension methods; use consistent newtonsoft serializer with openId authenticator
| 9464a05 | <span style="white-space:nowrap;">2022-06-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #5 from cortside/retry
| 9e9b339 | <span style="white-space:nowrap;">2022-06-15</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  set default to follow redirects to false/off
| 213e21c | <span style="white-space:nowrap;">2022-06-15</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/master, master) Merge pull request #6 from cortside/develop
| 97d077f | <span style="white-space:nowrap;">2022-07-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [feature/BOT-20220713] updated nuget packages
| 9fd9a43 | <span style="white-space:nowrap;">2022-07-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  handle git flow named branches
| 9eb54cc | <span style="white-space:nowrap;">2022-07-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #7 from cortside/feature/BOT-20220713
| 43c01d9 | <span style="white-space:nowrap;">2022-07-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [feature/BOT-20220713] updated nuget packages
| 005f0dc | <span style="white-space:nowrap;">2022-07-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [querybypost] add tests to ensure that 303 can be handled successfully
| e6e42b8 | <span style="white-space:nowrap;">2022-07-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [querybypost] override handling of 303 and 302 on POST
| 43498c5 | <span style="white-space:nowrap;">2022-07-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #8 from cortside/querybypost
| e481c40 | <span style="white-space:nowrap;">2022-07-29</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add the url for request to logged properties to make searching for responses easier
| 77faa7e | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add logging of request
| 6838eef | <span style="white-space:nowrap;">2022-08-01</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  adjust logging
| 0145d06 | <span style="white-space:nowrap;">2022-12-12</span> | <span style="white-space:nowrap;">Glen Kingston</span> |  [SVC-2031] Changed token to be empty instead of null when needed
| bcbe694 | <span style="white-space:nowrap;">2022-12-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  reflect changes to RestSharp in determining response success
| d14927e | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  minor cleanup
| c5c2667 | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'develop' into feature/SVC-2031
| 84ac55d | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Glen Kingston</span> |  [SVC-2031] Fixed test to check for empty due to new changes
| e8d6370 | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [SVC-2031] use constant instead of literal
| bd991de | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #9 from cortside/feature/SVC-2031
| 64c1cbe | <span style="white-space:nowrap;">2023-01-02</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  [feature/BOT-20230102] updated nuget packages
| 0952253 | <span style="white-space:nowrap;">2023-01-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/feature/BOT-20230102, feature/BOT-20230102) update helper scripts; update nuget packages
| fe878b8 | <span style="white-space:nowrap;">2023-01-03</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> develop, origin/develop, origin/HEAD) Merge pull request #10 from cortside/feature/BOT-20230102
****
