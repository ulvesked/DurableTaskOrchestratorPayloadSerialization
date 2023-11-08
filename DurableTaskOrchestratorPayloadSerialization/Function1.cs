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
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req, [DurableClient] DurableTaskClient durableTaskClient)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (req.Query["fromDate"] == null || !DateTime.TryParse(req.Query["fromDate"], out var fromDate)) {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            if (req.Query["toDate"] == null || !DateTime.TryParse(req.Query["toDate"], out var toDate)) {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            _logger.LogInformation("Executing orchestrator with fromDate {fromDate} and toDate {toDate}", fromDate, toDate);

            durableTaskClient.ScheduleNewMyOrchestratorInstanceAsync(new MyOrchestratorParameters
            {
                FromDate = fromDate,
                ToDate = toDate
            });

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
