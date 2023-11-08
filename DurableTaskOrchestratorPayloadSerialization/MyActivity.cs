
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurableTaskOrchestratorPayloadSerialization
{
    [DurableTask(nameof(MyActivity))]
    public class MyActivity : TaskActivity<MyActivityParameters, List<DateTime>>
    {
        private readonly ILogger<MyActivity> _logger;

        public MyActivity(ILogger<MyActivity> logger)
        {
            _logger = logger;
        }

        public override async Task<List<DateTime>> RunAsync(TaskActivityContext context, MyActivityParameters input)
        {
            _logger.LogInformation("Executing activity with fromDate {fromDate} and toDate {toDate}", input.FromDate, input.ToDate);
            if (input?.FromDate == null || input?.ToDate == null)
            {
                throw new ArgumentNullException("FromDate and ToDate are required");
            }
            if (input.FromDate > input.ToDate)
            {
                throw new ArgumentException("FromDate must be less than ToDate");
            }
            var result = new List<DateTime>();
            for (var i = input.FromDate; i <= input.ToDate; i = i.AddDays(1))
            {
                result.Add(i);
            }
            return result;
        }
    }

    public class MyActivityParameters
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
