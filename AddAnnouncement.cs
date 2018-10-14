using System;
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
    public static class Announcement
    {
        [FunctionName("AddAnnouncement")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            if (name == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                name = data?.name;
            }

            string storageAccountSetting = @"DefaultEndpointsProtocol=https;AccountName=mmebstorage;AccountKey=OVUxaTWk9L0gW2/7fjhX6gJZOAjH2me9zn6g1oQKxxQVU8TOadUaNp6JLMhGJdSi3E1MMk9ezQeo9SxXQOyzYA==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountSetting);

            
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("Announcements");

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            // Create a new customer entity.
            AnnouncementEntity announcement = new AnnouncementEntity(name, "Active");
            announcement.StartDate = DateTime.Now.ToShortDateString();

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(announcement);

            // Execute the insert operation.
            table.Execute(insertOperation);


            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }
    }
}
