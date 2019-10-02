using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DataPoints.Function {
    public static class DurableFunctionsOrchestrationCSharp {
        [FunctionName ("DurableFunctionsOrchestrationCSharp")]
        public static async Task<List<string>> RunOrchestrator (
            [OrchestrationTrigger] IDurableOrchestrationContext context) {
            var outputs = new List<string> ();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add (await context.CallActivityAsync<string> ("DurableFunctionsOrchestrationCSharp_Hello", "Tokyo"));
            outputs.Add (await context.CallActivityAsync<string> ("DurableFunctionsOrchestrationCSharp_Hello", "Seattle"));
            outputs.Add (await context.CallActivityAsync<string> ("DurableFunctionsOrchestrationCSharp_Hello", "London"));
            var entityId = new EntityId (nameof (Counter1), "myCounter1");

            // Synchronous call to the entity which returns a value
          //  int currentValue = await context.CallEntityAsync<int> (entityId, "Get");
           var currentValue=1;
            if (currentValue < 10) {
                // Asynchronous call which updates the value
            //  await context.CallEntityAsync<int>(entityId,"Add",1);
                  context.SignalEntity(entityId, "Add",1);
           } 
            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName ("DurableFunctionsOrchestrationCSharp_Hello")]
        public static string SayHello ([ActivityTrigger] string name, ILogger log) {
            log.LogInformation ($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName ("DurableFunctionsOrchestrationCSharp_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart (
            [HttpTrigger (AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req, [DurableClient] IDurableOrchestrationClient starter,
            ILogger log) {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync ("DurableFunctionsOrchestrationCSharp", null);

            log.LogInformation ($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse (req, instanceId);
        }

        [FunctionName ("Counter1")]
        public static void Counter1 ([EntityTrigger] IDurableEntityContext ctx) {
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