using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using MsdynTimeentry.BL;
using MsdynTimeentry.DataStorage;
using MsdynTimeentry.Http;
using MsdynTimeentry.Models;
using MsdynTimeentry.Properties;

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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.Post, Route = null)] HttpRequest req)
        {
            try
            {
                DateRange dateRange = await RequestsConverter.ConvertBodyTo<DateRange>(req, Encoding.UTF8.GetString(Resources.CreateMsdynTimeentriesRequestSchema));
                using BusinessLogicManager businessLogicManager = new BusinessLogicManager(dataStorageFactory.Create());
                int createdDatesCount = businessLogicManager.CreateNotExistsTimeentriesFromRange(dateRange.StartOn, dateRange.EndOn);
                return new OkObjectResult("Timeentries created: " + createdDatesCount.ToString());
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
