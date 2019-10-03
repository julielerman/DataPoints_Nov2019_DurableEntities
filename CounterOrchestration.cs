using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DataPoints.Function {
    public static class CounterOrchestration {
      [FunctionName ("CounterOrchestration")]
      public static async Task Run (
        [OrchestrationTrigger] IDurableOrchestrationContext context) {
        var entityId = new EntityId (nameof (Entity), "myCounter2");

        // Synchronous call to the entity which returns a value
        int currentValue = await context.CallEntityAsync<int>(entityId, "Get");
        if (currentValue < 10) {
          // Asynchronous call which updates the value
          context.SignalEntity(entityId, "Add", 1);
        }
      }
    }
      public class Entity {

        [FunctionName ("Entity")]
        public static void Counter ([EntityTrigger] IDurableEntityContext ctx) {
          int currentValue = ctx.GetState<int> ();

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
    }