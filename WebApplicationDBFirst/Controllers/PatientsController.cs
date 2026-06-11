using Microsoft.AspNetCore.Mvc;
using WebApplicationDBFirst.DTOs;
using WebApplicationDBFirst.Service;

namespace WebApplicationDBFirst.Controllers;

[ApiController]
[Route("api/[controller]")]

public class PatientsController(IDbService service) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetPatients([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var patients = await service.GetPatientsAsync(search, cancellationToken);
        return Ok(patients);
    }

    [HttpPost("{pesel}/bedassignments")]

    public async Task<IActionResult> AssignBed(string pesel, [FromBody] CreateBedAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await service.CreateBedAssignmentsAsync(pesel, request, cancellationToken);
            return Created("api/bedassignments", response);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
}