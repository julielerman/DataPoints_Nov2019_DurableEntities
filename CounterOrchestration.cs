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
using Newtonsoft.Json.Serialization;

namespace DataPoints.Function {
    public static class CounterOrchestration {
      [FunctionName ("CounterOrchestration")]
      public static async Task<string> RunOrchestartor (
        [OrchestrationTrigger] IDurableOrchestrationContext context) {
        var entityId = new EntityId (nameof (Entity), "key");
       context.SignalEntity(entityId, "Add", 2);
        // Synchronous call to the entity which returns a value
        var currentValue = await context.CallEntityAsync<int>(entityId, "Add",1);
        var x=3;
        return "xyz";
        // if (currentValue < 10) {
        //   // Asynchronous call which updates the value
        //   context.SignalEntity(entityId, "Add", 1);
        // }
      }
    
      public class Entity {

        [FunctionName ("Entity")]
        public static void Counter ([EntityTrigger] IDurableEntityContext ctx) {
                  var currentValue = ctx.GetState<int>();

             switch (ctx.OperationName.ToLowerInvariant ()) {
            case "add":
              int amount = ctx.GetInput<int> ();
              currentValue += amount;
              break;
            case "reset":
              currentValue = 0;
              break;
            case "get":
              ctx.Return (currentValue);
              break;
          }

          ctx.SetState (currentValue);
        }
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