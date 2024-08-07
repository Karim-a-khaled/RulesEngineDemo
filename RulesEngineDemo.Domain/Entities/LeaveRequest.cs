﻿namespace RulesEngineDemo.Domain.Entities;

public class LeaveRequest
{
    public int Id { get; set; }
    
    public int EmployeeId { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public bool IsApproved { get; set; }

    public bool IsFileUploaded { get; set; }
}
