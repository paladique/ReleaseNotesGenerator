using System.Net;
using Octokit;
using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;


public static async Task Run(HttpRequestMessage req, TraceWriter log)
{
    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Extract github comment from request body
    string releaseBody = data?.release?.body;
    string releaseName = data?.release?.name;

    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("jgblob"));
    var blobClient = storageAccount.CreateCloudBlobClient();
    var container = blobClient.GetContainerReference("releases");
    var blob = container.GetBlockBlobReference(releaseName + ".md" );

    string txtIssues = await GetReleaseDetails(IssueTypeQualifier.Issue);
    string txtPulls = await GetReleaseDetails(IssueTypeQualifier.PullRequest);
    
    var stuff =  String.Format("# {0} \n {1} \n\n" + "# Issues Closed:" + txtIssues + "\n\n# Changes Merged:" + txtPulls, releaseName, releaseBody);

    await blob.UploadTextAsync(stuff);    
}

public static async Task<string> GetReleaseDetails(IssueTypeQualifier type)
{
    var github = new GitHubClient(new ProductHeaderValue(Environment.GetEnvironmentVariable("ReleaseNotes")));
    var twoWeeks = DateTime.Now.Subtract(TimeSpan.FromDays(14));
    var request = new SearchIssuesRequest();

    request.Repos.Add(Environment.GetEnvironmentVariable("Repo"));
    request.Type = type;
    request.Created = new DateRange(twoWeeks, SearchQualifierOperator.GreaterThan);

    var issues = await github.Search.SearchIssues(request);

    string searchResults = string.Empty;
    foreach(Issue x in issues.Items)
    {
      searchResults += String.Format("\n - [{0}]({1})", x.Title, x.HtmlUrl);
    }

    return searchResults;
}