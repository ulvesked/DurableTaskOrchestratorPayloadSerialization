using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurableTaskOrchestratorPayloadSerialization
{
    [DurableTask(nameof(MyOrchestrator))]
    public class MyOrchestrator : TaskOrchestrator<MyOrchestratorParameters, MyOrchestratorResult>
    {
        public async override Task<MyOrchestratorResult> RunAsync(TaskOrchestrationContext context, MyOrchestratorParameters input)
        {
            var _logger = context.CreateReplaySafeLogger<MyOrchestrator>();
            if (input?.FromDate == null || input?.ToDate == null)
            {
                throw new ArgumentNullException("FromDate and ToDate are required");
            }
            _logger.LogInformation("MyOrchestrator.RunAsync: fromDate {fromDate}, toDate {toDate}", input.FromDate, input.ToDate);
            var data = await context.CallMyActivityAsync(new MyActivityParameters
            {
                FromDate = input.FromDate,
                ToDate = input.ToDate,
            });
            return new MyOrchestratorResult
            {
                FromDate = input.FromDate,
                ToDate = input.ToDate,
                Data = data
            };
        }
    }
    public class MyOrchestratorParameters
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public class MyOrchestratorResult
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<DateTime> Data { get; set; }
    }
}
