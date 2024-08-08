using Microsoft.AspNetCore.Mvc;
using RulesEngineDemo.Api.Extensions;
using RulesEngineDemo.Domain.Entities;
using RulesEngineDemo.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

app.MapPost("/approve-leave", async (ILeaveRequestService leaveRequestService, [FromBody]Employee employee) =>
{
    string result = await leaveRequestService.ApproveLeaveRequestAsync(employee);
    return result.Contains("Leave Approved") ? Results.Ok(result) : Results.BadRequest(result);
});

app.Run();
