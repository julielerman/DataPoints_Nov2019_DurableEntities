using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DataPoints.Function
{
    public static class CounterClearer
    {
        [FunctionName("CounterClearer")]
        public static async Task<int> RunOrchestartor(
          [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var entityId = new EntityId(nameof(Counter), "key");
            context.SignalEntity(entityId, "Reset");
            return await context.CallEntityAsync<int>(entityId, "Get");
        }
        [FunctionName("HttpTriggerClear")]
        public static async Task<IActionResult> Http(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
          [DurableClient] IDurableClient client)
        {
            string id = await client.StartNewAsync("CounterClearer", null);
            return client.CreateCheckStatusResponse(req, id);
        }
    }

}
