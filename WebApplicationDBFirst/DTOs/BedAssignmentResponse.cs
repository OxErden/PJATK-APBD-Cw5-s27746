namespace WebApplicationDBFirst.DTOs;

public record BedAssignmentResponse(
    int Id,
    DateTime From,
    DateTime? To,
    BedResponse Bed);