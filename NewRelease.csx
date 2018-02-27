using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ICollector<string> releaseQueueItem, TraceWriter log)
{
    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Extract github comment from request body
    string releaseBody = data?.release?.body;
    string releaseName = data?.release?.name;

    var releaseDetails = String.Format("{0},{1}", releaseName, releaseBody);

    releaseQueueItem.Add(releaseDetails);    
 
    return req.CreateResponse(HttpStatusCode.OK, "Works!");
}
