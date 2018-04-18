using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace RunPipelineAzureFunctions
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            // Set variables
            string tenantID = ""; // your tenant ID
            string applicationId = ""; // your application ID
            string authenticationKey = ""; // your authentication key for the application
            string subscriptionId = ""; // your subscription ID where the data factory resides
            string resourceGroup = ""; // your resource group where the data factory resides
            string dataFactoryName = ""; // specify the name of data factory to create. It must be globally unique
            string pipelineName = "";    // name of the pipeline

            // Authenticate and create a data factory management client
            var context = new AuthenticationContext("https://login.windows.net/" + tenantID);
            ClientCredential cc = new ClientCredential(applicationId, authenticationKey);
            AuthenticationResult result = context.AcquireTokenAsync("https://management.azure.com/", cc).Result;
            ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);
            var client = new DataFactoryManagementClient(cred) { SubscriptionId = subscriptionId };

            CreateRunResponse runResponse = client.Pipelines.CreateRunWithHttpMessagesAsync(resourceGroup, dataFactoryName, pipelineName).Result.Body;
            //log.Info("Pipline run ID: " + runResponse.RunId);

            return req.CreateResponse(HttpStatusCode.OK, "Pipeline run ID: " + runResponse.RunId);         
        }
    }
}
