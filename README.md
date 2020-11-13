# Release Notes Generator
Tool for generating a release notes document for projects hosted on GitHub.

[![Deploy to Azure](https://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/?repository=https://github.com/paladique/ReleaseNotesGenerator)

## What's It Do?
The generator is a [function app](https://docs.microsoft.com/azure/azure-functions/functions-overview?WT.mc_id=academic-0000-jasmineg) containing a [GitHub webhook](https://docs.microsoft.com/azure/azure-functions/functions-create-github-webhook-triggered-function?WT.mc_id=academic-0000-jasmineg) function that creates a Markdown file whenever a new release is created, using [Azure Blob Storage](https://azure.microsoft.com/services/storage/blobs?WT.mc_id=academic-0000-jasmineg).

## What do I need to make one?
You'll need an [Azure](https://azure.microsoft.com/free?WT.mc_id=academic-0000-jasmineg) account, and a GitHub account with an active repository.

## How do I make one?
If you'd like to build it from scratch,follow this [tutorial](Tutorial.md).

### Filling out the deploy settings:
To replicate the services used on your Azure subscription, click the "Deploy to Azure" button above and fill out the relevant configuration settings.

**Storage Account Connection String** You'll need a storage account with a blob container called `releases`. Follow the first two sections of the following walkthrough on how to [create a container](https://docs.microsoft.com/azure/storage/blobs/storage-quickstart-blobs-portal?WT.mc_id=academic-0000-jasmineg). In the storage account menu, navigate to **Access keys** and copy the connection string for this field.

**GitHub App Name** [Create an OAuth App on Github](https://developer.github.com/apps/building-oauth-apps/creating-an-oauth-app/). Paste the App name into this field.

## Resources on services used
* [Introduction to Azure Functions](https://docs.microsoft.com/azure/azure-functions/functions-overview?WT.mc_id=academic-0000-jasmineg)
* [Azure Functions triggers and bindings concepts](https://docs.microsoft.com/azure/azure-functions/functions-triggers-bindings?WT.mc_id=academic-0000-jasmineg)
* [Azure Functions C# script (.csx) developer reference](https://docs.microsoft.com/azure/azure-functions/functions-reference-csharp?WT.mc_id=academic-0000-jasmineg)
* [OctoKit.NET](https://octokit.github.io/)