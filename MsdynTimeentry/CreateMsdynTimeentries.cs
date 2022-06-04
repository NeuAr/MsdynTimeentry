using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using MsdynTimeentry.DataStorage;
using Newtonsoft.Json;

namespace MsdynTimeentry
{
    public class CreateMsdynTimeentries
    {
        private readonly IDataStorageFactory dataStorageFactory;

        public CreateMsdynTimeentries(IDataStorageFactory dataStorageFactory)
        {
            this.dataStorageFactory = dataStorageFactory ?? throw new ArgumentNullException(nameof(dataStorageFactory), "Data storage factory is null");
        }

        [FunctionName("CreateMsdynTimeentries")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
