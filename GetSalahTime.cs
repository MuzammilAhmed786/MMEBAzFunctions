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
using Newtonsoft.Json;

namespace MMEBAzFunctions
{
    public static class GetSalahTime
    {
        [FunctionName("GetSalahTime")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string storageAccountSetting = @"DefaultEndpointsProtocol=https;AccountName=mmebstorage;AccountKey=OVUxaTWk9L0gW2/7fjhX6gJZOAjH2me9zn6g1oQKxxQVU8TOadUaNp6JLMhGJdSi3E1MMk9ezQeo9SxXQOyzYA==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountSetting);


            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("SalahTimes");

            TableQuery<SalahTimeEntity> query = new TableQuery<SalahTimeEntity>();

            List<SalahTimeEntity> salahTimeEntities = new List<SalahTimeEntity>();

            // Print the fields for each Staff.
            foreach (SalahTimeEntity entity in table.ExecuteQuery(query))
            {
                if (!string.IsNullOrEmpty(entity.JsonForm))
                {

                    //foreach (var item in jsonResponse)
                    //{
                    //    JObject jData = (JObject)item;

                    //    //SalahTimeEntity rowsResult = JsonConvert.DeserializeObject<SalahTimeEntity>(jData.ToString());
                    //    //testDataList.Add(rowsResult);

                    //    salahTimeEntity.JsonForm = jData.ToString();
                    //}

                    SalahTimeEntity salahTimeEntity = JsonConvert.DeserializeObject<SalahTimeEntity>(entity.JsonForm);
                    salahTimeEntities.Add(salahTimeEntity);
                }
            }

            return req.CreateResponse(HttpStatusCode.OK, salahTimeEntities);

        }
    }
}
