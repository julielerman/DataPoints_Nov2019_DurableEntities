using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            var entityId = new EntityId (nameof (CounterEntity), "myCounter1");

            // Synchronous call to the entity which returns a value
           
          var currentValue=await context.CallEntityAsync<int>(entityId,"Get");
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
    }
   public class CounterEntity
{
    [JsonProperty("value")]
    public int CurrentValue { get; set; }

    public void Add(int amount) => this.CurrentValue += amount;
    
    public void Reset() => this.CurrentValue = 0;
    
    public int Get() => this.CurrentValue;

    [FunctionName(nameof(CounterEntity))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx)
        => ctx.DispatchAsync<CounterEntity>();
}
    }
