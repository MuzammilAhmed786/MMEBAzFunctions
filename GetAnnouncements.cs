using System.Collections.Generic;
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
    public static class GetAnnouncements
    {
        [FunctionName("GetAnnouncements")]
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

            // Create a new customer entity.
            AnnouncementEntity announcement = new AnnouncementEntity();

            // Construct the query operation for all customer entities where PartitionKey="Active".
            TableQuery<AnnouncementEntity> query = new TableQuery<AnnouncementEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Active"));

            //List<string> announcements = new List<string>();
            List<AnnouncementModel> announcements = new List<AnnouncementModel>();

            // Print the fields for each announcement.
            foreach (AnnouncementEntity entity in table.ExecuteQuery(query))
            {
                if(!string.IsNullOrEmpty(entity.Announcement))
                {
                    AnnouncementModel announcementModel = new AnnouncementModel();
                    announcementModel.Announcement = entity.Announcement ?? string.Empty;
                    announcementModel.StartDate = entity.StartDate;
                    announcements.Add(announcementModel);
                }
            }

            return req.CreateResponse(HttpStatusCode.OK, announcements);
        }
    }

    public class AnnouncementModel
    {
        public string Announcement { get; set; }

        public string StartDate { get; set; }
    }

}
