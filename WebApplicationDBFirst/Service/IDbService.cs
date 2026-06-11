using WebApplicationDBFirst.DTOs;

namespace WebApplicationDBFirst.Service;

public interface IDbService
{
    Task<List<PatientResponse>> GetPatientsAsync(string? search, CancellationToken cancellationToken);
}