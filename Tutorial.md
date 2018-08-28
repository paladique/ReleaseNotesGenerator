## Tutorial

### Create a Blob Container
1. Navigate to the Azure Portal and create a storage account. See the [Create a storage account quickstart](https://docs.microsoft.com/en-us/azure/storage/common/storage-quickstart-create-account?tabs=portal#create-a-general-purpose-storage-account?WT.mc_id=demo-functions-jasmineg) to get started. 
2. Navigate to the new storage account, navigate to the **Blob Service** section, select **Browse Blobs**, then click the **Add Container** button at the top to create a blob container named `releases`. See section on how to [create a container](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-portal#create-a-container?WT.mc_id=demo-functions-jasmineg) for more information.
3. In the same storage account menu, navigate to **Access keys** and copy the connection string.

### Create and Configure a GitHub Webhook Triggered Function
1. Create a function app. See section on how to [create a function app](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-azure-function#create-a-function-app?WT.mc_id=demo-functions-jasmineg) to get started.
2. Navigate to the new function, from the overview, click and open **Application settings**, scroll to and click **+ Add new setting**. Name the setting `StorageAccountConnectionString` and paste the copied connection string into the value field. Click **Save**
3. In the function app, add a C# GitHub webhook function. See section on how to [Create a GitHub webhook triggered function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-github-webhook-triggered-function#create-a-github-webhook-triggered-function?WT.mc_id=demo-functions-jasmineg) to get started.

4. Replace starter code with the following:

```csharp
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

    // Extract github release from request body
    string releaseBody = data?.release?.body;
    string releaseName = data?.release?.name;
    string repositoryName = data?.repository?.full_name;

    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("StorageAccountConnectionString"));
    var blobClient = storageAccount.CreateCloudBlobClient();
    var container = blobClient.GetContainerReference("releases");
    var blob = container.GetBlockBlobReference(releaseName + ".md" );

    string txtIssues = await GetReleaseDetails(IssueTypeQualifier.Issue, repositoryName);
    string txtPulls = await GetReleaseDetails(IssueTypeQualifier.PullRequest, repositoryName);
    
    var text = String.Format("# {0} \n {1} \n\n" + "# Issues Closed:" + txtIssues + "\n\n# Changes Merged:" + txtPulls, releaseName, releaseBody);

    await blob.UploadTextAsync(text);    
}

public static async Task<string> GetReleaseDetails(IssueTypeQualifier type, string repoName)
{
    //Connect to client with OAuth App
    var github = new GitHubClient(new ProductHeaderValue(Environment.GetEnvironmentVariable("ReleaseNotes")));
    var twoWeeks = DateTime.Now.Subtract(TimeSpan.FromDays(14));
    var range = new DateRange(twoWeeks, SearchQualifierOperator.GreaterThanOrEqualTo);
    var request = new SearchIssuesRequest();

    request.Repos.Add(repoName);
    request.Type = type;

    //Find Issues or PRs closed or merged within the past 14 days in specified Repo
    if (type == IssueTypeQualifier.Issue)
    {
        request.Closed = range;
    }
    else
    {
        request.Merged = range;
    }

    var issues = await github.Search.SearchIssues(request);

    //Iterate and format text 
    string searchResults = string.Empty;
    foreach(Issue x in issues.Items)
    {
      searchResults += String.Format("\n - [{0}]({1})", x.Title, x.HtmlUrl);
    }

    return searchResults;
}
```

1. In your new function, copy the url by clicking click **</> Get function URL**, and save for later. Repeat for **</> Get GitHub secret**. You will use these values to configure the webhook in GitHub.

### Configure GitHub Webhook
1. Navigate to GitHub and select the repository to use with webhook. Navgiate to the repository's settings.
2. In the menu on the left of the repository settings, select webhooks and click **add a webhook** button.
3. Follow the table to configure your settings:

| Setting | Suggested value | Description |
|---|---|---|
| **Payload URL** | Copied value | Use the value returned by  **</> Get function URL**. |
| **Content type** | application/json | The function expects a JSON payload. |
| **Secret**   | Copied value | Use the value returned by  **</> Get GitHub secret**. |
| **Event triggers** | Let me select individual events | We only want to trigger on release events.  |

4. Click **add webhook**.
5. Navigate to your GitHub user settings, then to **Developer Applications**. Click **New OAuth App** and create an app with a homepage url and callback url of your choice, as they will not be used in the app. Copy and save the application name for later use.
6. Go back to the portal and to the function app **Application settings**, scroll to and click **+ Add new setting**. Name the setting `ReleaseNotes` and paste the copied GitHub OAuth App name into the value field. Click **Save**.

### Test the application
Create a new release in the repository. Fill out the required fields and click **Publish release**. The generated blob will be a markdown file named as the release title.
Monitor and review the functions' execution history in the **Monitor** context menu of the function.
To run the function again without creating another release, go to the configured webhook in GitHub to redeliver it.