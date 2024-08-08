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
   ```sh
2. Restore dependencies and build the solution:
    ```sh
    dotnet restore
    dotnet build
    ```sh
### Running The Application
    ```sh
    cd RulesEngineDemo.Api
    dotnet run
    ```sh
### Domain Layer

### Entities

#### Employee

**Represents an employee within the system.**

```csharp
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsMarried { get; set; }
    public bool HasNewBorn { get; set; }
    public int YearsOfService { get; set; }
}
```csharp
### LeaveRequest Entity

#### LeaveRequest

**Represents a leave request submitted by an employee.**

```csharp
public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsApproved { get; set; }
}
```csharp

### ILeaveRequestService Interface

#### ILeaveRequestService

**Defines the contract for a service that handles leave request approval.**

```csharp
public interface ILeaveRequestService
{
    Task<string> ApproveLeaveRequestAsync(Employee employee);
}
```csharp

# Elsa Workflows vs Microsoft Rules Engine

## Elsa Workflow
### Purpose: Orchestrate and automate complex business processes.
### * Key Feature
    * Versatile definition methods: C#, JSON, visual designer.
    * Handles both short and long-running processes.
    * User-friendly web-based designer for visual workflow creation.
    * Supports complex workflow structures: composition, parallel execution.
    * Manages workflow versions for evolution.
    * Flexible persistence options for data storage.

## Microsoft Rules Engine
### Purpose: Evaluate conditions and execute actions based on defined rules.
### * Key Feature
    * Rule definition using JSON for easy integration.
    * Optimized for high-performance rule evaluation.
    * Customizable with custom operators and actions.
    * Integrates seamlessly with various .NET applications.

## Focus

    * **Elsa Workflows:** Broad workflow management.
    * **Microsoft Rules Engine:** Rule definition and execution.

## Complexity

    * **Elsa Workflows:** Suitable for complex workflows.
    * **Microsoft Rules Engine:** Simplifies rule-based logic.

## Design

    * **Elsa Workflows:** Visual designer for workflows.
    * **Microsoft Rules Engine:** JSON-based rule definitions.

### Usage
The application exposes API endpoints for interacting with the rules engine. You can test these endpoints using tools like Postman or cURL.