# release-notes-generator
Azure Function Webhook that generates release notes

Add to queue trigger's function.json:

```
    {
      "name": "blobContainer",
      "type": "blob",
      "direction": "inout",
      "path": "container-name/*",
      "connection": "blobConnectionName"
    }
```
