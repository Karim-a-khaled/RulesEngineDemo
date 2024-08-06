using RulesEngine.Models;
using RulesEngineDemo.Domain.Entities;
using RulesEngineDemo.Domain.Interfaces;
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

    public async Task<bool> ApproveLeaveRequestAsync(Employee employee)
    {
        RuleParameter ruleParams = new RuleParameter("employee", employee);
        List<RuleResultTree> results = await _rulesEngine.ExecuteAllRulesAsync("FatherhoodLeaveRule", ruleParams);

        bool isApproved = results.All(r => r.IsSuccess);

        LeaveRequest leaveRequest = new LeaveRequest
        {
            EmployeeId = employee.Id,
            StartDate = DateTime.UtcNow,
            EndDate = endDate,
            IsApproved = isApproved
        };

        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();

        return isApproved;
    }
}
