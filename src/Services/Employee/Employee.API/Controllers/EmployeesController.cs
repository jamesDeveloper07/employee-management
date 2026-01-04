using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Employee.Application.Commands;
using Employee.Application.Queries;
using Employee.Application.DTOs;

namespace Employee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IMediator mediator, ILogger<EmployeesController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all employees
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all employees", Description = "Returns a list of all employees")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<EmployeeDto>))]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
    {
        _logger.LogInformation("Getting all employees");
        var query = new GetAllEmployeesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get employee by ID", Description = "Returns a single employee by ID")]
    [SwaggerResponse(200, "Success", typeof(EmployeeDto))]
    [SwaggerResponse(404, "Employee not found")]
    public async Task<ActionResult<EmployeeDto>> GetById(Guid id)
    {
        _logger.LogInformation("Getting employee by ID: {Id}", id);
        var query = new GetEmployeeByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
        {
            _logger.LogWarning("Employee not found: {Id}", id);
            return NotFound(new { message = "Employee not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get active employees
    /// </summary>
    [HttpGet("active")]
    [SwaggerOperation(Summary = "Get active employees", Description = "Returns a list of active employees")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<EmployeeDto>))]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetActive()
    {
        _logger.LogInformation("Getting active employees");
        var query = new GetActiveEmployeesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get employees by department
    /// </summary>
    [HttpGet("department/{departmentId:guid}")]
    [SwaggerOperation(Summary = "Get employees by department", Description = "Returns employees from a specific department")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<EmployeeDto>))]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetByDepartment(Guid departmentId)
    {
        _logger.LogInformation("Getting employees by department: {DepartmentId}", departmentId);
        var query = new GetEmployeesByDepartmentQuery(departmentId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new employee
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create employee", Description = "Creates a new employee")]
    [SwaggerResponse(201, "Employee created", typeof(EmployeeDto))]
    [SwaggerResponse(400, "Invalid data")]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeCommand command)
    {
        _logger.LogInformation("Creating new employee: {FirstName} {LastName}", command.FirstName, command.LastName);

        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning("Validation failed for create employee: {Errors}", ex.Errors);
            return BadRequest(new { errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing employee
    /// </summary>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update employee", Description = "Updates an existing employee")]
    [SwaggerResponse(200, "Employee updated", typeof(EmployeeDto))]
    [SwaggerResponse(404, "Employee not found")]
    [SwaggerResponse(400, "Invalid data")]
    public async Task<ActionResult<EmployeeDto>> Update(Guid id, [FromBody] UpdateEmployeeCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        _logger.LogInformation("Updating employee: {Id}", id);

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning("Validation failed for update employee: {Errors}", ex.Errors);
            return BadRequest(new { errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }) });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Employee not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete an employee
    /// </summary>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete employee", Description = "Deletes an employee")]
    [SwaggerResponse(204, "Employee deleted")]
    [SwaggerResponse(404, "Employee not found")]
    public async Task<ActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deleting employee: {Id}", id);

        try
        {
            var command = new DeleteEmployeeCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Employee not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Activate an employee
    /// </summary>
    [HttpPatch("{id:guid}/activate")]
    [SwaggerOperation(Summary = "Activate employee", Description = "Activates an inactive employee")]
    [SwaggerResponse(200, "Employee activated")]
    [SwaggerResponse(404, "Employee not found")]
    public async Task<ActionResult> Activate(Guid id)
    {
        _logger.LogInformation("Activating employee: {Id}", id);

        try
        {
            var command = new ActivateEmployeeCommand(id);
            await _mediator.Send(command);
            return Ok(new { message = "Employee activated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Employee not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating employee");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Deactivate an employee
    /// </summary>
    [HttpPatch("{id:guid}/deactivate")]
    [SwaggerOperation(Summary = "Deactivate employee", Description = "Deactivates an active employee")]
    [SwaggerResponse(200, "Employee deactivated")]
    [SwaggerResponse(404, "Employee not found")]
    public async Task<ActionResult> Deactivate(Guid id)
    {
        _logger.LogInformation("Deactivating employee: {Id}", id);

        try
        {
            var command = new DeactivateEmployeeCommand(id);
            await _mediator.Send(command);
            return Ok(new { message = "Employee deactivated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Employee not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating employee");
            return BadRequest(new { message = ex.Message });
        }
    }
}
