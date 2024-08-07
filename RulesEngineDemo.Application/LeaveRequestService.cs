using RulesEngine.Models;
using RulesEngineDemo.Domain.Entities;
using RulesEngineDemo.Domain.Interfaces;
using RulesEngineDemo.Infrastructure.Data;
using System.Text.Json;

namespace RulesEngineDemo.Application;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly LeaveDbContext _context;
    private readonly RulesEngine.RulesEngine _rulesEngine;

    public LeaveRequestService(LeaveDbContext dbContext)
    {
        _context = dbContext;
        Workflow workflow = JsonSerializer.Deserialize<Workflow>(File.ReadAllText("Rules/leave-rules.json"));
        _rulesEngine = new RulesEngine.RulesEngine(new[] { workflow }, null);
    }

    public async Task<string> ApproveLeaveRequestAsync(Employee employee)
    {
        var results = await ExecuteRulesAsync(employee);

        bool isApproved = results.All(r => r.IsSuccess);

        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = startDate;

        string approvalMessage = "Leave Not Approved";

        if (isApproved)
        {
            var successfulRule = results.First(r => r.IsSuccess);
            (TimeSpan duration, string message) = GetLeaveDurationAndMessage(successfulRule.Rule.SuccessEvent);

            if (duration != TimeSpan.Zero)
            {
                endDate = startDate.Add(duration);
                approvalMessage = message;
            }
        }

        await CreateLeaveRequestAsync(employee.Id, startDate, endDate, isApproved);

        return approvalMessage;
    }

    private async Task<List<RuleResultTree>> ExecuteRulesAsync(Employee employee)
    {
        RuleParameter employeeRuleParams = new RuleParameter("employee", employee);
        return await _rulesEngine.ExecuteAllRulesAsync("FatherhoodLeaveRule", employeeRuleParams);
    }

    private (TimeSpan, string) GetLeaveDurationAndMessage(string successEvent)
    {
        var leaveDurations = new Dictionary<string, (TimeSpan, string)>
        {
            { "Fatherhood Leave Approved for 1 Month", (TimeSpan.FromDays(30), "Leave Approved for 30 Days") },
            { "Fatherhood Leave Approved for 3 Weeks", (TimeSpan.FromDays(21), "Leave Approved for 21 Days") },
            { "Fatherhood Leave Approved for 10 Days", (TimeSpan.FromDays(10), "Leave Approved for 10 Days") },
            { "Fatherhood Leave Approved for 1 Week", (TimeSpan.FromDays(7), "Leave Approved for 7 Days") }
        };

        return leaveDurations.TryGetValue(successEvent, out var result) ? result : (TimeSpan.Zero, "Leave Not Approved");
    }

    private async Task CreateLeaveRequestAsync(int employeeId, DateTime startDate, DateTime endDate, bool isApproved)
    {
        LeaveRequest leaveRequest = new LeaveRequest
        {
            EmployeeId = employeeId,
            StartDate = startDate,
            EndDate = endDate,
            IsApproved = isApproved,
        };

        await _context.LeaveRequests.AddAsync(leaveRequest);
        await _context.SaveChangesAsync();
    }
}
