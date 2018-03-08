//Create a new C# QueueTrigger, replace with original code, upload project.json (in this repo)

#r "Microsoft.WindowsAzure.Storage"
using System;
using Microsoft.WindowsAzure.Storage.Blob;
using Octokit;

public static async Task Run(string myQueueItem, CloudBlockBlob blobContainer, TraceWriter log)
{
    var releaseDetails = myQueueItem.Split(',');
    var releaseName = releaseDetails[0];
    var releaseBody = releaseDetails[1];
    
    string txtIssues = await GetReleaseDetails(IssueTypeQualifier.Issue);
    string txtPulls = await GetReleaseDetails(IssueTypeQualifier.PullRequest);

    var myBlobContainer = blobContainer.Container;
    var releaseText =  String.Format("# {0} \n {1} \n\n" + "# Issues Closed:" + txtIssues + "\n\n# Changes Merged:" + txtPulls, releaseName,releaseBody);
    var blob = myBlobContainer.GetAppendBlobReference(releaseName + ".md" );

    await blob.UploadTextAsync(releaseText);    
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
