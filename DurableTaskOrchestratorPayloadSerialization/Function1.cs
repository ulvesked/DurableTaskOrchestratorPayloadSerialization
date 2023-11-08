using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Microsoft.DurableTask;

namespace DurableTaskOrchestratorPayloadSerialization
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req, [DurableClient] DurableTaskClient durableTaskClient)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (req.Query["fromDate"] == null || !DateTime.TryParse(req.Query["fromDate"], out var fromDate)) {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            if (req.Query["toDate"] == null || !DateTime.TryParse(req.Query["toDate"], out var toDate)) {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            _logger.LogInformation("Executing orchestrator with fromDate {fromDate} and toDate {toDate}", fromDate, toDate);

           var orchestration = await durableTaskClient.ScheduleNewMyOrchestratorInstanceAsync(new MyOrchestratorParameters
            {
                FromDate = fromDate,
                ToDate = toDate
            });

            var result = await durableTaskClient.WaitForInstanceCompletionAsync(orchestration, true, req.FunctionContext.CancellationToken);
            
            response.WriteString(result.SerializedOutput);

            return response;
        }
    }
}
