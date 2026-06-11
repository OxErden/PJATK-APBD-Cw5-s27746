namespace WebApplicationDBFirst.DTOs;

public record BedResponse(
    int Id,
    BedTypeResponse BedType,
    RoomResponse Room);