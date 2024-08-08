# RulesEngineDemo Documentation

## Overview
The RulesEngineDemo is a demonstration project for implementing a rules engine using C#. This project showcases how to build a flexible and maintainable rules engine that can be used to evaluate complex business rules.

## Repository Structure
The repository is organized into the following main folders:

- **RulesEngineDemo.Api**: Contains the API layer of the application.
- **RulesEngineDemo.Application**: Includes application logic and services.
- **RulesEngineDemo.Domain**: Defines the domain models and entities.
- **RulesEngineDemo.Infrastructure**: Implements infrastructure and data access.

## Getting Started
### Prerequisites
- .NET SDK (version X.X)
- IDE (e.g., Visual Studio, Visual Studio Code)

### Installation
1. Clone the repository:
   ```sh
   git clone https://github.com/Karim-a-khaled/RulesEngineDemo.git
   cd RulesEngineDemo
2. Restore dependencies and build the solution:
    ```sh
    dotnet restore
    dotnet build
3. Running The Application
    ```sh
    cd RulesEngineDemo.Api
    dotnet run
### Domain Layer
#### Contains the core business entities, behaviors, and logic. This layer is independent of any external frameworks or libraries.

1. ### Entities

    * **Employee** 

        *Represents an employee within the system.*

        ```csharp
        public class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsMarried { get; set; }
            public bool HasNewBorn { get; set; }
            public int YearsOfService { get; set; }
        }
        ```

    * **LeaveRequest.**

        *Represents a leave request submitted by an employee.*

        ```csharp
        public class LeaveRequest
        {
            public int Id { get; set; }
            public int EmployeeId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool IsApproved { get; set; }
        }
        ```

2. ### Interfaces

    * **ILeaveRequestService.**

    **Defines the contract for a service that handles leave request approval.**

    ```csharp
    public interface ILeaveRequestService
    {
        Task<string> ApproveLeaveRequestAsync(Employee employee);
    }
    ```

### Application Layer
#### Defines the use cases and orchestrates the interaction between the domain and other layers.

1. **LeaveRequestService.**
    * #### Class Definition and Dependencies
            This code defines a class called LeaveRequestService that implements the ILeaveRequestService interface. The class has two private fields: _context (a LeaveDbContext object) and _rulesEngine (a RulesEngine.RulesEngine object). The constructor of the class takes a LeaveDbContext object as a parameter, which is used to initialize the _context field.
            
        ```csharp
        public class LeaveRequestService : ILeaveRequestService
        {
            private readonly LeaveDbContext _context;
            private readonly RulesEngine.RulesEngine _rulesEngine;

            public LeaveRequestService(LeaveDbContext dbContext)
            {
                _context = dbContext;
                // ...
            }
        }
        ```
    * #### Rules Engine Initialization
            This part of the code reads a JSON file located at the specified file path, deserializes the contents into a Workflow object, and then creates a new RulesEngine.RulesEngine object using the deserialized workflow. If the deserialized workflow is null or empty, an InvalidOperationException is thrown.

        ```csharp
        string filePath = "C:\\Users\\Karim Khaled\\source\\repos\\RulesEngineDemo\\RulesEngineDemo.Application\\Rules\\leave-rules.json";
        var jsonContent = File.ReadAllText(filePath);
        var workflow = JsonSerializer.Deserialize<Workflow>(jsonContent);

        if (workflow == null || workflow.Rules == null || !workflow.Rules.Any())
        {
            throw new InvalidOperationException("Deserialized workflow is null or empty.");
        }

        _rulesEngine = new RulesEngine.RulesEngine(new[] { workflow }, null);
        ```

    * #### Approving a Leave Request
        ```csharp
        public async Task<string> ApproveLeaveRequestAsync(Employee employee)
        {
            var results = await ExecuteRulesAsync(employee);

            bool isApproved = results.Any(r => r.IsSuccess);
            string approvalMessage = "Leave Not Approved";

            if (isApproved)
            {
                var successfulRule = results.First(r => r.IsSuccess);
                (TimeSpan duration, string message) = GetLeaveDurationAndMessage(successfulRule.Rule.SuccessEvent);

                if (duration != TimeSpan.Zero)
                {
                    var startDate = DateTime.UtcNow;
                    var endDate = startDate.Add(duration);

                    await CreateLeaveRequestAsync(employee.Id, startDate, endDate, isApproved);
                    approvalMessage = message;
                }
            }

            return approvalMessage;
        }
        ```

    This method, ApproveLeaveRequestAsync, takes an Employee object as a parameter and performs the following steps:

    1- Executes the rules engine asynchronously by calling the ExecuteRulesAsync method.
    
    2- Checks if any of the executed rules were successful (i.e., the leave request is approved).
    
    3- If the leave request is approved, it retrieves the leave duration and message from the GetLeaveDurationAndMessage method.
    
    4- If the leave duration is not zero, it creates a new LeaveRequest object and saves it to the database using the CreateLeaveRequestAsync method.
    5- Finally, it returns the approval message.

    6- Executing Rules Asynchronously:
        ```csharp
        private async Task<List<RuleResultTree>> ExecuteRulesAsync(Employee employee)
        {
            var employeeRuleParams = new RuleParameter("employee", employee);
            return await _rulesEngine.ExecuteAllRulesAsync("FatherhoodLeaveRule", employeeRuleParams);
        }
        ```
    This private method, ExecuteRulesAsync, takes an Employee object as a parameter and creates a RuleParameter object with the employee as the value. It then calls the ExecuteAllRulesAsync method of the _rulesEngine object, passing the rule name "FatherhoodLeaveRule" and the RuleParameter object. The method returns a list of RuleResultTree objects, which represent the results of executing the rules.

    * #### Retrieving Leave Duration and Message:

    ```csharp
    private (TimeSpan, string) GetLeaveDurationAndMessage(string successEvent)
    {
        var leaveDurations = new Dictionary<string, (TimeSpan, string)>
        {
            // ...
        };

        return leaveDurations.TryGetValue(successEvent, out var result) ? result : (TimeSpan.Zero, "Leave Not Approved");
    }
    ```
    This private method, GetLeaveDurationAndMessage, takes a successEvent string as a parameter and retrieves the corresponding leave duration and approval message from a dictionary of leave durations. If the successEvent is not found in the dictionary, the method returns a TimeSpan of zero and the message "Leave Not Approved".

    * #### Creating a Leave Request
    ```csharp
    private async Task CreateLeaveRequestAsync(int employeeId, DateTime startDate, DateTime endDate, bool isApproved)
    {
        var leaveRequest = new LeaveRequest
        {
            EmployeeId = employeeId,
            StartDate = startDate,
            EndDate = endDate,
            IsApproved = isApproved
        };

        await _context.LeaveRequests.AddAsync(leaveRequest);
        await _context.SaveChangesAsync();
    }
    ```

    This private method, CreateLeaveRequestAsync, takes an employeeId, startDate, endDate, and isApproved flag as parameters. It creates a new LeaveRequest object with the provided values, adds it to the _context.LeaveRequests collection, and then saves the changes to the database asynchronously.

    Overall, this code implements a service for handling leave requests, including executing rules to determine if a leave request should be approved, and creating a new leave request in the database if the request is approved.


### Infrastructure Layer
#### Includes the implementation of external dependencies, such as databases, web services, and message queues.

1. ### Creating DB Context
    ```csharp
    public class LeaveDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        public LeaveDbContext(DbContextOptions<LeaveDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
    ```

### Presentation Layer
#### Handles the user interface and user interactions.

# Elsa Workflows vs Microsoft Rules Engine

## Elsa Workflow
#### Purpose: Orchestrate and automate complex business processes.

#### Key Feature
    * Versatile definition methods: C#, JSON, visual designer.
    * Handles both short and long-running processes.
    * User-friendly web-based designer for visual workflow creation.
    * Supports complex workflow structures: composition, parallel execution.
    * Manages workflow versions for evolution.
    * Flexible persistence options for data storage.

## Microsoft Rules Engine
#### Purpose: Evaluate conditions and execute actions based on defined rules.
####  Key Feature
    * Rule definition using JSON for easy integration.
    * Optimized for high-performance rule evaluation.
    * Customizable with custom operators and actions.
    * Integrates seamlessly with various .NET applications.

#### Focus

    * Elsa Workflows:** Broad workflow management.
    * Microsoft Rules Engine:** Rule definition and execution.

#### Complexity

    * Elsa Workflows:** Suitable for complex workflows.
    * Microsoft Rules Engine:** Simplifies rule-based logic.

#### Design

    * Elsa Workflows:** Visual designer for workflows.
    * Microsoft Rules Engine:** JSON-based rule definitions.

#### Usage
The application exposes API endpoints for interacting with the rules engine. You can test these endpoints using tools like Postman or cURL.