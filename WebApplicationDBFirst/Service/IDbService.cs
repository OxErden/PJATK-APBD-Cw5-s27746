using WebApplicationDBFirst.DTOs;

namespace WebApplicationDBFirst.Service;

public interface IDbService
{
    public Task<List<PatientResponse>> GetPatientsAsync(string? search, CancellationToken cancellationToken);
    public Task <BedAssignmentResponse> CreateBedAssignmentsAsync(string pesel, CreateBedAssignmentRequest request,CancellationToken cancellationToken);
}