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

    public async Task<BedAssignmentResponse> CreateBedAssignmentsAsync(string pesel, CreateBedAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        
        if (request.To.HasValue && request.To <= request.From)
        {
            throw new ArgumentException("Start date cannot be later or the same as the end date");
        }
        
        var patientExists = await ctx.Patients.AnyAsync(p => p.Pesel == pesel, cancellationToken);
        if (!patientExists)
        {
            throw new KeyNotFoundException("Patient with such pesel not found");
        }

        var wardExists = await ctx.Wards.FirstOrDefaultAsync(w => w.Name == request.Ward, cancellationToken);
        if (wardExists is null)
        {
            throw new KeyNotFoundException("Ward not found");
        }

        var bedTypeExists = await ctx.BedTypes.FirstOrDefaultAsync(bt => bt.Name == request.BedType, cancellationToken);
        if (bedTypeExists is null)
        {
            throw new KeyNotFoundException("BedType not found");
        }
        
        var sqlMaxDate = new DateTime(9999, 12, 31, 23, 59, 59);
        var requestFrom = request.From;
        var requestTo = request.To ?? sqlMaxDate;

        var availableBed = await ctx.Beds
            .Include(b => b.BedType)
            .Include(b => b.Room)
            .ThenInclude(r => r.Ward)
            .Where(b => b.BedTypeId == bedTypeExists.Id && b.Room.WardId == wardExists.Id)
            .Where(b => !b.BedAssignments.Any(
                (ba => (ba.To == null || request.From <= ba.To) &&
                       (request.To == null || ba.From <= request.To)))).FirstOrDefaultAsync(cancellationToken);

        if (availableBed is null)
        {
            throw new KeyNotFoundException("No available be in this ward for the requested period");
        }

        var assign = new BedAssignment
        {
            PatientPesel = pesel,
            BedId = availableBed.Id,
            From = request.From,
            To = request.To,
        };
        
        ctx.BedAssignments.Add(assign);
        await ctx.SaveChangesAsync(cancellationToken);
        
        return new BedAssignmentResponse(
            assign.Id,
            assign.From,
            assign.To,
            new BedResponse(
                availableBed.Id,
                new BedTypeResponse(
                    availableBed.BedType.Id,
                    availableBed.BedType.Name,
                    availableBed.BedType.Description),
                new RoomResponse(
                    availableBed.Room.Id,
                    availableBed.Room.HasTv,
                    new WardResponse(
                        availableBed.Room.Ward.Id,
                        availableBed.Room.Ward.Name,
                        availableBed.Room.Ward.Description))));

    }
}

