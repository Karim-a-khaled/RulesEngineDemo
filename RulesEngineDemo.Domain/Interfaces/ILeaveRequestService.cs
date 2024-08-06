using RulesEngineDemo.Domain.Entities;

namespace RulesEngineDemo.Domain.Interfaces;

public interface ILeaveRequestService
{
    Task<bool> ApproveLeaveRequestAsync(Employee employee);
}