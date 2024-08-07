using RulesEngine.Models;
using RulesEngineDemo.Domain.Entities;
using RulesEngineDemo.Domain.Interfaces;
using RulesEngineDemo.Infrastructure.Data;
using System;
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
        RuleParameter employeeRuleParams = new RuleParameter("employee", employee);
        List<RuleResultTree> results = await _rulesEngine.ExecuteAllRulesAsync("FatherhoodLeaveRule", employeeRuleParams);

        bool isApproved = results.All(r => r.IsSuccess);

        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = startDate;

        // Mapping of success event messages to their respective durations
        var leaveDurations = new Dictionary<string, TimeSpan>
        {
            { "Fatherhood Leave Approved for 1 Month", TimeSpan.FromDays(30) },
            { "Fatherhood Leave Approved for 3 Weeks", TimeSpan.FromDays(21) },
            { "Fatherhood Leave Approved for 10 Days", TimeSpan.FromDays(10) },
            { "Fatherhood Leave Approved for 1 Week", TimeSpan.FromDays(7) }
        };

        string approvalMessage = "Leave Not Approved";

        if (isApproved)
        {
            var successfulRule = results.First(r => r.IsSuccess);
            string successEvent = successfulRule.Rule.SuccessEvent;

            if (leaveDurations.TryGetValue(successEvent, out TimeSpan duration))
            {
                endDate = startDate.Add(duration);
                approvalMessage = $"Leave Approved for {duration.Days} Days";
            }
        }

        LeaveRequest leaveRequest = new LeaveRequest
        {
            EmployeeId = employee.Id,
            StartDate = startDate,
            EndDate = endDate,
            IsApproved = isApproved,
        };

        await _context.LeaveRequests.AddAsync(leaveRequest);
        await _context.SaveChangesAsync();

        return approvalMessage;
    }

}
