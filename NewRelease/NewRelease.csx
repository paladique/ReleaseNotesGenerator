using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ICollector<string> releaseQueueItem, TraceWriter log)
{
    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Extract github comment from request body
    string releaseBody = data?.release?.body;
    string releaseName = data?.release?.name;    
    string repositoryName = data?.repository?.full_name;

    //Format message and send to queue
    var releaseDetails = string.Format("{0}|{1}|{2}", releaseName, releaseBody, repositoryName);
    releaseQueueItem.Add(releaseDetails);    
}
