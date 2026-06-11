using System.ComponentModel.DataAnnotations;

namespace WebApplicationDBFirst.DTOs;

public record CreateBedAssignmentRequest(
    DateTime From,
    DateTime? To,
    [MaxLength(100)] string BedType,
    [MaxLength(100)] string Ward);