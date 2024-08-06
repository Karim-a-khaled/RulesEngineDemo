using RulesEngineDemo.Api.Extensions;
using RulesEngineDemo.Domain.Entities;
using RulesEngineDemo.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

app.MapPost("/approve-leave", async (ILeaveRequestService leaveRequestService, Employee employee) =>
{
    var result = await leaveRequestService.ApproveLeaveRequestAsync(employee);
    return result ? Results.Ok("Leave Approved") : Results.BadRequest("Leave Not Approved");
});

app.Run();
