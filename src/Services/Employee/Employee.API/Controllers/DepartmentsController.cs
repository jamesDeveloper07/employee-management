using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Employee.Domain.Repositories;
using Employee.Application.DTOs;
using AutoMapper;
using Employee.Domain.Entities;

namespace Employee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(
        IDepartmentRepository departmentRepository,
        IMapper mapper,
        ILogger<DepartmentsController> logger)
    {
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all departments
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all departments", Description = "Returns a list of all departments")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<DepartmentDto>))]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll()
    {
        _logger.LogInformation("Getting all departments");
        var departments = await _departmentRepository.GetAllAsync();
        var departmentsDto = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        return Ok(departmentsDto);
    }

    /// <summary>
    /// Get active departments
    /// </summary>
    [HttpGet("active")]
    [SwaggerOperation(Summary = "Get active departments", Description = "Returns a list of active departments")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<DepartmentDto>))]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetActive()
    {
        _logger.LogInformation("Getting active departments");
        var departments = await _departmentRepository.GetActiveAsync();
        var departmentsDto = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        return Ok(departmentsDto);
    }

    /// <summary>
    /// Get department by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get department by ID", Description = "Returns a single department by ID")]
    [SwaggerResponse(200, "Success", typeof(DepartmentDto))]
    [SwaggerResponse(404, "Department not found")]
    public async Task<ActionResult<DepartmentDto>> GetById(Guid id)
    {
        _logger.LogInformation("Getting department by ID: {Id}", id);
        var department = await _departmentRepository.GetByIdAsync(id);

        if (department == null)
        {
            _logger.LogWarning("Department not found: {Id}", id);
            return NotFound(new { message = "Department not found" });
        }

        var departmentDto = _mapper.Map<DepartmentDto>(department);
        return Ok(departmentDto);
    }

    /// <summary>
    /// Create a new department
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create department", Description = "Creates a new department")]
    [SwaggerResponse(201, "Department created", typeof(DepartmentDto))]
    [SwaggerResponse(400, "Invalid data")]
    public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentRequest request)
    {
        _logger.LogInformation("Creating new department: {Name}", request.Name);

        try
        {
            var department = Department.Create(request.Name, request.Description);
            _departmentRepository.Add(department);
            await _departmentRepository.GetByIdAsync(department.Id); // Trigger save via UnitOfWork in real scenario

            var departmentDto = _mapper.Map<DepartmentDto>(department);
            return CreatedAtAction(nameof(GetById), new { id = department.Id }, departmentDto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation failed for create department: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating department");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing department
    /// </summary>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update department", Description = "Updates an existing department")]
    [SwaggerResponse(200, "Department updated", typeof(DepartmentDto))]
    [SwaggerResponse(404, "Department not found")]
    [SwaggerResponse(400, "Invalid data")]
    public async Task<ActionResult<DepartmentDto>> Update(Guid id, [FromBody] UpdateDepartmentRequest request)
    {
        _logger.LogInformation("Updating department: {Id}", id);

        try
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                return NotFound(new { message = "Department not found" });
            }

            department.Update(request.Name, request.Description);
            _departmentRepository.Update(department);

            var departmentDto = _mapper.Map<DepartmentDto>(department);
            return Ok(departmentDto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation failed for update department: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating department");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a department
    /// </summary>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete department", Description = "Deletes a department")]
    [SwaggerResponse(204, "Department deleted")]
    [SwaggerResponse(404, "Department not found")]
    public async Task<ActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deleting department: {Id}", id);

        try
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                return NotFound(new { message = "Department not found" });
            }

            _departmentRepository.Delete(department);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting department");
            return BadRequest(new { message = ex.Message });
        }
    }
}

// DTOs for Department requests
public record CreateDepartmentRequest(string Name, string Description);
public record UpdateDepartmentRequest(string Name, string Description);
