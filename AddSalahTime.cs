using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MMEBAzFunctions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MMEBAzFunctions
{
    public static class AddSalahTime
    {
        [FunctionName("AddSalahTime")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string salahtTime = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "salah", true) == 0)
                .Value;

            string iqamaTime = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "iqama", true) == 0)
                .Value;

            if (salahtTime == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                salahtTime = data?.name;
            }

            if (iqamaTime == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                iqamaTime = data?.iqamaTime;
            }

            string storageAccountSetting = @"DefaultEndpointsProtocol=https;AccountName=mmebstorage;AccountKey=OVUxaTWk9L0gW2/7fjhX6gJZOAjH2me9zn6g1oQKxxQVU8TOadUaNp6JLMhGJdSi3E1MMk9ezQeo9SxXQOyzYA==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountSetting);


            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("SalahTimes");
            table.CreateIfNotExists();

            SalahTimeEntity salahTime = ReadJsonFile();

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(salahTime);

            // Execute the insert operation.
            table.Execute(insertOperation);

            return req.CreateResponse(HttpStatusCode.OK);
        }

        private static SalahTimeEntity ReadJsonFile()
        {
            SalahTimeEntity salahTimeEntity = new SalahTimeEntity("First", "Active");

            try
            {
                string currentFile = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                Console.WriteLine(currentFile);
                string dir = new FileInfo(currentFile).Directory.Parent.FullName;
                string pathToJson = Path.Combine(dir, "SalahTime.json");
                Console.WriteLine(pathToJson);

                using (StreamReader file = File.OpenText(@pathToJson))
                using (Newtonsoft.Json.JsonTextReader reader = new Newtonsoft.Json.JsonTextReader(file))
                {
                    JArray jsonResponse = null;
                    try
                    {
                        jsonResponse = (JArray)JToken.ReadFrom(reader);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);

                       // jsonResponse = (JArray)JToken.Parse(ResourceReader.GetEmbeddedResourceString(Assembly.GetExecutingAssembly(), "TestData.json"));
                    }

                    //List<SalahTimeEntity> testDataList = new List<SalahTimeEntity>();

                    //foreach (var item in jsonResponse)
                    //{
                    //    JObject jData = (JObject)item;

                    //    //SalahTimeEntity rowsResult = JsonConvert.DeserializeObject<SalahTimeEntity>(jData.ToString());
                    //    //testDataList.Add(rowsResult);

                    //    salahTimeEntity.JsonForm = jData.ToString();
                    //}

                    salahTimeEntity.JsonForm = jsonResponse.ToString();


                }

                return salahTimeEntity;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                //Console.WriteLine(ex.StackTrace);

                return null;
            }

        }
    }
}
