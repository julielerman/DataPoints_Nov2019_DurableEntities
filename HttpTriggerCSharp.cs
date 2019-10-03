using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace DataPoints.Function
{
  public static class HttpTriggerCSharp
  {
    [FunctionName("CounterClientAdd")]
    public static Task CounterClientAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
          [DurableClient] IDurableClient client)
    {
      var entityId = new EntityId(nameof(Counter), "myCounter");
      int amount = int.Parse(req.Query["amount"]);
      return client.SignalEntityAsync(entityId, "Add", amount);
    }

    [FunctionName("CounterClientGet")]
    public static async Task<HttpResponseMessage> CounterClientGet(
                  [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestMessage req,
          [DurableClient] IDurableClient client)
    {

      var entityId = new EntityId(nameof(Counter), "myCounter");
      //var currentState = await client.ReadEntityStateAsync<CounterClass>(entityId); //<--for classes, not functins
      var currentValue = await client.ReadEntityStateAsync<JObject>(entityId);

      return req.CreateResponse(HttpStatusCode.OK, currentValue.EntityState);
    }


    [FunctionName("Counter")]
    public static void Counter([EntityTrigger] IDurableEntityContext ctx)
    {
      int currentValue = ctx.GetState<int>();

      switch (ctx.OperationName.ToLowerInvariant())
      {
        case "add":
          int amount = ctx.GetInput<int>();
          currentValue += amount;
          break;
        case "reset":
          currentValue = 0;
          break;
        case "get":
          ctx.Return(currentValue);
          break;
      }

      ctx.SetState(currentValue);
    }
  }
}
