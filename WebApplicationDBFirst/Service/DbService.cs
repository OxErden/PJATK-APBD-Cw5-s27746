using Microsoft.EntityFrameworkCore;
using WebApplicationDBFirst.DTOs;
using WebApplicationDBFirst.Models;

namespace WebApplicationDBFirst.Service;

public class DbService(HospitalContext ctx) : IDbService
{
    public async Task<List<PatientResponse>> GetPatientsAsync(string? search, CancellationToken cancellationToken)
    {
        var result = ctx.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            result = result.Where(p =>
                EF.Functions.Like(p.FirstName, pattern) ||
                EF.Functions.Like(p.LastName, pattern));

        }

        return await result.Select(p => new PatientResponse(
            p.Pesel,
            p.FirstName,
            p.LastName,
            p.Age,
            p.Sex ? "Male" : "Female",
            p.Admissions.Select(a => new AdmissionResponse(
                a.Id,
                a.AdmissionDate,
                a.DischargeDate,
                new WardResponse(
                    a.Ward.Id,
                    a.Ward.Name,
                    a.Ward.Description)
            )).ToList(),
            p.BedAssignments.Select(ba => new BedAssignmentResponse(
                    ba.Id,
                    ba.From,
                    ba.To,
                    new BedResponse(
                        ba.Bed.Id,
                        new BedTypeResponse(
                            ba.Bed.BedType.Id,
                            ba.Bed.BedType.Name,
                            ba.Bed.BedType.Description),
                        new RoomResponse(
                            ba.Bed.Room.Id,
                            ba.Bed.Room.HasTv,
                            new WardResponse(
                                ba.Bed.Room.Ward.Id,
                                ba.Bed.Room.Ward.Name,
                                ba.Bed.Room.Ward.Description)
                        )
                    )
                )
            ).ToList()
        )).ToListAsync(cancellationToken);
    }
}

