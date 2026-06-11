namespace WebApplicationDBFirst.DTOs;

public record AdmissionResponse(
    int Id,
    DateTime AdmissionDate,
    DateTime? DischargeDate,
    WardResponse Ward);