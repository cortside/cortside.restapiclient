using System;
using Cortside.MockServer;
using Cortside.MockServer.Builder;
using Cortside.RestApiClient.Tests.Clients.CatalogApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Cortside.RestApiClient.Tests.Mocks {
    public class TestMock : IMockHttpMock {
        public void Configure(MockHttpServer server) {
            var rnd = new Random();

            server.WireMockServer
                .Given(
                    Request.Create().WithPath("/api/v1/items/search")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(303)
                        .WithHeader("Content-Type", "application/json")
                        .WithHeader("Location", "/api/v1/items/search")
                );

            server.WireMockServer
                .Given(
                    Request.Create().WithPath("/api/v1/items/search")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(r => JsonConvert.SerializeObject(new CatalogItem() {
                            ItemId = Guid.NewGuid(),
                            Name = $"Item {r.PathSegments[3]}",
                            Sku = r.PathSegments[3],
                            UnitPrice = new decimal(rnd.Next(10000) / 100.0)
                        }))
                );

            server.WireMockServer
                .Given(
                    Request.Create().WithPath("/api/v1/items")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(201)
                        .WithHeader("Content-Type", "application/json")
                        .WithHeader("Location", "/api/v1/items/1234")
                        .WithBody(_ => JsonConvert.SerializeObject(new CatalogItem() {
                            ItemId = Guid.NewGuid(),
                            Name = "Item 1234",
                            Sku = "1234",
                            UnitPrice = 15.99M
                        }))
                );

            server.WireMockServer
                .Given(
                    Request.Create().WithPath("/api/v1/items/*")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(r => JsonConvert.SerializeObject(new CatalogItem() {
                            ItemId = Guid.NewGuid(),
                            Name = $"Item {r.PathSegments[3]}",
                            Sku = r.PathSegments[3],
                            UnitPrice = new decimal(rnd.Next(10000) / 100.0)
                        }))
                );

            server.WireMockServer
                .Given(
                    Request.Create().WithPath("/api/v1/timeout")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(@"{ ""msg"": ""Hello I'm a little bit slow!"" }")
                        .WithDelay(TimeSpan.FromSeconds(10))
                );

            server.WireMockServer
                .Given(
                    Request.Create().WithPath("/api/v1/302")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(302)
                        .WithHeader("Content-Type", "application/json")
                        .WithHeader("Location", "/api/v1/temp302")
                );

            server.WireMockServer
                .Given(
                    Request.Create().WithPath("/api/v1/jsonmodelmismatch")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_ => JsonConvert.SerializeObject(new JObject {
                            ["ItemId"] = null
                        }))
                );

            server.WireMockServer
                .Given(
                    Request.Create().WithPath("/users/cortside/repos")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_ =>
"""
[
{
  "id": 451569361,
  "node_id": "R_kgDOGupm0Q",
  "name": "cortside.restapiclient",
  "full_name": "cortside/cortside.restapiclient",
  "private": false,
  "owner": {
    "login": "cortside",
    "id": 32287038,
    "node_id": "MDEyOk9yZ2FuaXphdGlvbjMyMjg3MDM4",
    "avatar_url": "https://avatars.githubusercontent.com/u/32287038?v=4",
    "gravatar_id": "",
    "url": "https://api.github.com/users/cortside",
    "html_url": "https://github.com/cortside",
    "followers_url": "https://api.github.com/users/cortside/followers",
    "following_url": "https://api.github.com/users/cortside/following{/other_user}",
    "gists_url": "https://api.github.com/users/cortside/gists{/gist_id}",
    "starred_url": "https://api.github.com/users/cortside/starred{/owner}{/repo}",
    "subscriptions_url": "https://api.github.com/users/cortside/subscriptions",
    "organizations_url": "https://api.github.com/users/cortside/orgs",
    "repos_url": "https://api.github.com/users/cortside/repos",
    "events_url": "https://api.github.com/users/cortside/events{/privacy}",
    "received_events_url": "https://api.github.com/users/cortside/received_events",
    "type": "Organization",
    "site_admin": false
  },
  "html_url": "https://github.com/cortside/cortside.restapiclient",
  "description": null,
  "fork": false,
  "url": "https://api.github.com/repos/cortside/cortside.restapiclient",
  "forks_url": "https://api.github.com/repos/cortside/cortside.restapiclient/forks",
  "keys_url": "https://api.github.com/repos/cortside/cortside.restapiclient/keys{/key_id}",
  "collaborators_url": "https://api.github.com/repos/cortside/cortside.restapiclient/collaborators{/collaborator}",
  "teams_url": "https://api.github.com/repos/cortside/cortside.restapiclient/teams",
  "hooks_url": "https://api.github.com/repos/cortside/cortside.restapiclient/hooks",
  "issue_events_url": "https://api.github.com/repos/cortside/cortside.restapiclient/issues/events{/number}",
  "events_url": "https://api.github.com/repos/cortside/cortside.restapiclient/events",
  "assignees_url": "https://api.github.com/repos/cortside/cortside.restapiclient/assignees{/user}",
  "branches_url": "https://api.github.com/repos/cortside/cortside.restapiclient/branches{/branch}",
  "tags_url": "https://api.github.com/repos/cortside/cortside.restapiclient/tags",
  "blobs_url": "https://api.github.com/repos/cortside/cortside.restapiclient/git/blobs{/sha}",
  "git_tags_url": "https://api.github.com/repos/cortside/cortside.restapiclient/git/tags{/sha}",
  "git_refs_url": "https://api.github.com/repos/cortside/cortside.restapiclient/git/refs{/sha}",
  "trees_url": "https://api.github.com/repos/cortside/cortside.restapiclient/git/trees{/sha}",
  "statuses_url": "https://api.github.com/repos/cortside/cortside.restapiclient/statuses/{sha}",
  "languages_url": "https://api.github.com/repos/cortside/cortside.restapiclient/languages",
  "stargazers_url": "https://api.github.com/repos/cortside/cortside.restapiclient/stargazers",
  "contributors_url": "https://api.github.com/repos/cortside/cortside.restapiclient/contributors",
  "subscribers_url": "https://api.github.com/repos/cortside/cortside.restapiclient/subscribers",
  "subscription_url": "https://api.github.com/repos/cortside/cortside.restapiclient/subscription",
  "commits_url": "https://api.github.com/repos/cortside/cortside.restapiclient/commits{/sha}",
  "git_commits_url": "https://api.github.com/repos/cortside/cortside.restapiclient/git/commits{/sha}",
  "comments_url": "https://api.github.com/repos/cortside/cortside.restapiclient/comments{/number}",
  "issue_comment_url": "https://api.github.com/repos/cortside/cortside.restapiclient/issues/comments{/number}",
  "contents_url": "https://api.github.com/repos/cortside/cortside.restapiclient/contents/{+path}",
  "compare_url": "https://api.github.com/repos/cortside/cortside.restapiclient/compare/{base}...{head}",
  "merges_url": "https://api.github.com/repos/cortside/cortside.restapiclient/merges",
  "archive_url": "https://api.github.com/repos/cortside/cortside.restapiclient/{archive_format}{/ref}",
  "downloads_url": "https://api.github.com/repos/cortside/cortside.restapiclient/downloads",
  "issues_url": "https://api.github.com/repos/cortside/cortside.restapiclient/issues{/number}",
  "pulls_url": "https://api.github.com/repos/cortside/cortside.restapiclient/pulls{/number}",
  "milestones_url": "https://api.github.com/repos/cortside/cortside.restapiclient/milestones{/number}",
  "notifications_url": "https://api.github.com/repos/cortside/cortside.restapiclient/notifications{?since,all,participating}",
  "labels_url": "https://api.github.com/repos/cortside/cortside.restapiclient/labels{/name}",
  "releases_url": "https://api.github.com/repos/cortside/cortside.restapiclient/releases{/id}",
  "deployments_url": "https://api.github.com/repos/cortside/cortside.restapiclient/deployments",
  "created_at": "2022-01-24T17:43:28Z",
  "updated_at": "2022-06-14T18:04:17Z",
  "pushed_at": "2023-09-12T21:42:23Z",
  "git_url": "git://github.com/cortside/cortside.restapiclient.git",
  "ssh_url": "git@github.com:cortside/cortside.restapiclient.git",
  "clone_url": "https://github.com/cortside/cortside.restapiclient.git",
  "svn_url": "https://github.com/cortside/cortside.restapiclient",
  "homepage": null,
  "size": 220,
  "stargazers_count": 0,
  "watchers_count": 0,
  "language": "C#",
  "has_issues": true,
  "has_projects": true,
  "has_downloads": true,
  "has_wiki": true,
  "has_pages": false,
  "has_discussions": false,
  "forks_count": 1,
  "mirror_url": null,
  "archived": false,
  "disabled": false,
  "open_issues_count": 4,
  "license": {
    "key": "mit",
    "name": "MIT License",
    "spdx_id": "MIT",
    "url": "https://api.github.com/licenses/mit",
    "node_id": "MDc6TGljZW5zZTEz"
  },
  "allow_forking": true,
  "is_template": false,
  "web_commit_signoff_required": false,
  "topics": [],
  "visibility": "public",
  "forks": 1,
  "open_issues": 4,
  "watchers": 0,
  "default_branch": "develop"
},
{
  "id": 221544927,
  "node_id": "MDEwOlJlcG9zaXRvcnkyMjE1NDQ5Mjc=",
  "name": "cortside.sqlreportapi",
  "full_name": "cortside/cortside.sqlreportapi",
  "private": false,
  "owner": {
    "login": "cortside",
    "id": 32287038,
    "node_id": "MDEyOk9yZ2FuaXphdGlvbjMyMjg3MDM4",
    "avatar_url": "https://avatars.githubusercontent.com/u/32287038?v=4",
    "gravatar_id": "",
    "url": "https://api.github.com/users/cortside",
    "html_url": "https://github.com/cortside",
    "followers_url": "https://api.github.com/users/cortside/followers",
    "following_url": "https://api.github.com/users/cortside/following{/other_user}",
    "gists_url": "https://api.github.com/users/cortside/gists{/gist_id}",
    "starred_url": "https://api.github.com/users/cortside/starred{/owner}{/repo}",
    "subscriptions_url": "https://api.github.com/users/cortside/subscriptions",
    "organizations_url": "https://api.github.com/users/cortside/orgs",
    "repos_url": "https://api.github.com/users/cortside/repos",
    "events_url": "https://api.github.com/users/cortside/events{/privacy}",
    "received_events_url": "https://api.github.com/users/cortside/received_events",
    "type": "Organization",
    "site_admin": false
  },
  "html_url": "https://github.com/cortside/cortside.sqlreportapi",
  "description": null,
  "fork": false,
  "url": "https://api.github.com/repos/cortside/cortside.sqlreportapi",
  "forks_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/forks",
  "keys_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/keys{/key_id}",
  "collaborators_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/collaborators{/collaborator}",
  "teams_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/teams",
  "hooks_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/hooks",
  "issue_events_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/issues/events{/number}",
  "events_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/events",
  "assignees_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/assignees{/user}",
  "branches_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/branches{/branch}",
  "tags_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/tags",
  "blobs_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/git/blobs{/sha}",
  "git_tags_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/git/tags{/sha}",
  "git_refs_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/git/refs{/sha}",
  "trees_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/git/trees{/sha}",
  "statuses_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/statuses/{sha}",
  "languages_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/languages",
  "stargazers_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/stargazers",
  "contributors_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/contributors",
  "subscribers_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/subscribers",
  "subscription_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/subscription",
  "commits_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/commits{/sha}",
  "git_commits_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/git/commits{/sha}",
  "comments_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/comments{/number}",
  "issue_comment_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/issues/comments{/number}",
  "contents_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/contents/{+path}",
  "compare_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/compare/{base}...{head}",
  "merges_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/merges",
  "archive_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/{archive_format}{/ref}",
  "downloads_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/downloads",
  "issues_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/issues{/number}",
  "pulls_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/pulls{/number}",
  "milestones_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/milestones{/number}",
  "notifications_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/notifications{?since,all,participating}",
  "labels_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/labels{/name}",
  "releases_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/releases{/id}",
  "deployments_url": "https://api.github.com/repos/cortside/cortside.sqlreportapi/deployments",
  "created_at": "2019-11-13T20:24:19Z",
  "updated_at": "2022-03-17T13:54:43Z",
  "pushed_at": "2022-12-22T02:30:29Z",
  "git_url": "git://github.com/cortside/cortside.sqlreportapi.git",
  "ssh_url": "git@github.com:cortside/cortside.sqlreportapi.git",
  "clone_url": "https://github.com/cortside/cortside.sqlreportapi.git",
  "svn_url": "https://github.com/cortside/cortside.sqlreportapi",
  "homepage": null,
  "size": 291,
  "stargazers_count": 0,
  "watchers_count": 0,
  "language": "C#",
  "has_issues": true,
  "has_projects": true,
  "has_downloads": true,
  "has_wiki": true,
  "has_pages": false,
  "has_discussions": false,
  "forks_count": 1,
  "mirror_url": null,
  "archived": false,
  "disabled": false,
  "open_issues_count": 1,
  "license": {
    "key": "mit",
    "name": "MIT License",
    "spdx_id": "MIT",
    "url": "https://api.github.com/licenses/mit",
    "node_id": "MDc6TGljZW5zZTEz"
  },
  "allow_forking": true,
  "is_template": false,
  "web_commit_signoff_required": false,
  "topics": [],
  "visibility": "public",
  "forks": 1,
  "open_issues": 1,
  "watchers": 0,
  "default_branch": "develop"
}
]                       
"""
        )
                );
        }
    }
}
