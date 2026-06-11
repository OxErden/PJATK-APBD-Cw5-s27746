using Microsoft.AspNetCore.Mvc;
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
    
}