# BlobTail
Windows application to "tail" log blobs in Azure Storage.

The application monitors append blobs in an Azure blob container, and continually displays the text being appended. When a new blob is added to the container, the application automatically switches to that blob.

The application connects to an Azure storage account by connection string. You can obtain your connection string in your storage account in Azure portal under "Access Keys". 

Download the installer here: https://github.com/auzick/BlobTail/tree/master/BlobTail/Installer 
 
This app is particularly useful if you're using my CloudLogging library (https://github.com/auzick/CloudLogging) for logging from your applications. I use this as my application logging mechanism for Azure Web App services.
