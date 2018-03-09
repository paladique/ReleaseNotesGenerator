#r "Microsoft.WindowsAzure.Storage"
using System;
using Microsoft.WindowsAzure.Storage.Blob;
using Octokit;

public static async Task Run(string myQueueItem, CloudBlockBlob blobContainer, TraceWriter log)
{
    //Parse queue message
    var releaseDetails = myQueueItem.Split('|');
    var releaseName = releaseDetails[0];
    var releaseBody = releaseDetails[1];
    var repoName = releaseDetails[2];

    //Get issues and pull requests from release repo
    string txtIssues = await GetReleaseDetails(IssueTypeQualifier.Issue, repoName);
    string txtPulls = await GetReleaseDetails(IssueTypeQualifier.PullRequest, repoName);

    //Get reference to blob container
    var myBlobContainer = blobContainer.Container;
    var releaseText =  String.Format("# {0} \n {1} \n\n" + "# Issues Closed:" + txtIssues + "\n\n# Changes Merged:" + txtPulls, releaseName,releaseBody);

    //Create a blob with the release name as the file name and append formatted release notes
    var blob = myBlobContainer.GetAppendBlobReference(releaseName + ".md" );
    await blob.UploadTextAsync(releaseText);
}

public static async Task<string> GetReleaseDetails(IssueTypeQualifier type, string repo)
{
    //Create GitHub Client
    var github = new GitHubClient(new ProductHeaderValue(Environment.GetEnvironmentVariable("ReleaseNotesApp")));
    var span = DateTime.Now.Subtract(TimeSpan.FromDays(14));
    var request = new SearchIssuesRequest();

    request.Repos.Add(repo);
    request.Type = type;

    //Search closed issues or merged pull requests
    var qualifier = (type == IssueTypeQualifier.Issue) ? request.Closed : request.Merged;
    qualifier = new DateRange(span, SearchQualifierOperator.GreaterThan);
    var issues = await github.Search.SearchIssues(request);

    string searchResults = string.Empty;
    foreach(Issue x in issues.Items)
    {
      searchResults += String.Format("\n - [{0}]({1})", x.Title, x.HtmlUrl);
    }

    return searchResults;
}
