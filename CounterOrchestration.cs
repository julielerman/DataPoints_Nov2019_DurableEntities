using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DataPoints.Function
{
    public static class CounterOrchestration
    {
        [FunctionName("CounterOrchestration")]
        public static async Task<int> RunOrchestartor(
          [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
           // string output;
            var entityId = new EntityId(nameof(Counter), "key");
            //var currentValue = await context.CallEntityAsync<int>(entityId, "Get");
            //if (currentValue < 10)
            //{
                // Asynchronous call which updates the value
                context.SignalEntity(entityId, "Add", 1);
            //}
            var finalValue= await context.CallEntityAsync<int>(entityId, "Get");
            //return $"{finalValue}";
            //return output;
            return finalValue;
           

        }
        [FunctionName("HttpTrigger")]
        public static async Task<IActionResult> Http(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
          [DurableClient] IDurableClient client)
        {
            string id = await client.StartNewAsync("CounterOrchestration", null);
            return client.CreateCheckStatusResponse(req, id);
        }
    }

}
