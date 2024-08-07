using RulesEngineDemo.Domain.Entities;

namespace RulesEngineDemo.Domain.Interfaces;

public interface ILeaveRequestService
{
    Task<string> ApproveLeaveRequestAsync(Employee employee);
}