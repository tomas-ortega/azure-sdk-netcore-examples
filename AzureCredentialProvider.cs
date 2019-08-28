using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace azure_sdk_practices_netcore
{
    class AzureCredentialProvider {
        public Microsoft.Azure.Management.Fluent.IAzure LoginAzure() {
            var credentials = SdkContext.AzureCredentialsFactory
                            .FromFile("C:/Users/admin12345/projects/azure-sdk-practices-netcore/authFile.json");

            Microsoft.Azure.Management.Fluent.IAzure azure = Azure
                        .Configure()
                        .WithLogLevel(Microsoft.Azure.Management.ResourceManager.Fluent.Core.HttpLoggingDelegatingHandler.Level.Basic)
                        .Authenticate(credentials)
                        .WithDefaultSubscription();

            return azure;
        }
    }
}