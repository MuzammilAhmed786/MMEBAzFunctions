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
    public static class GetStaff
    {
        [FunctionName("GetStaff")]
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
            CloudTable table = tableClient.GetTableReference("Staffs");

            // Create a new customer entity.
            StaffEntity Staff = new StaffEntity();

            // Construct the query operation for all customer entities where PartitionKey="Active".
            TableQuery<StaffEntity> query = new TableQuery<StaffEntity>();

            //List<string> Staffs = new List<string>();
            List<StaffModel> Staffs = new List<StaffModel>();

            // Print the fields for each Staff.
            foreach (StaffEntity entity in table.ExecuteQuery(query))
            {
                if (!string.IsNullOrEmpty(entity.StaffName))
                {
                    StaffModel StaffModel = new StaffModel();
                    StaffModel.StaffName = entity.StaffName ?? string.Empty;
                    StaffModel.StartDate = entity.StartDate ?? string.Empty;
                    StaffModel.ImageURL = entity.ImageURL ?? string.Empty;
                    StaffModel.Designation = entity.Designation ?? string.Empty;
                    Staffs.Add(StaffModel);
                }
            }

            return req.CreateResponse(HttpStatusCode.OK, Staffs);
        }
    }

    public class StaffModel
    {
        public string StaffName { get; set; }

        public string StartDate { get; set; }

        public string ImageURL { get; set; }

        public string Designation { get; set; }


    }
}
