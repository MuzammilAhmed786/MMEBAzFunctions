using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MMEBAzFunctions.Entities;

namespace MMEBAzFunctions
{
    public static class DeleteSalahTime
    {
        [FunctionName("DeleteSalahTime")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string salahtTime = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "salah", true) == 0)
                .Value;


            if (salahtTime == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                salahtTime = data?.name;
            }

            

            string storageAccountSetting = @"DefaultEndpointsProtocol=https;AccountName=mmebstorage;AccountKey=OVUxaTWk9L0gW2/7fjhX6gJZOAjH2me9zn6g1oQKxxQVU8TOadUaNp6JLMhGJdSi3E1MMk9ezQeo9SxXQOyzYA==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountSetting);


            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable deleteTable = tableClient.GetTableReference("SalahTimes");
            deleteTable.DeleteIfExists();

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
